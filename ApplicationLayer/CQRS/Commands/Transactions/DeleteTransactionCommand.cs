using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Transactions
{
    public record DeleteTransactionCommand(Guid TransactionId) : IRequest<Result<Guid>>;

    internal class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Transaction> _transactions;
        private readonly IGenericRepository<Customer> _customers;

        private readonly IUnitOfWork _unitOfWork;

        public DeleteTransactionCommandHandler(IGenericRepository<Transaction> transactions, IGenericRepository<Customer> customers, IUnitOfWork unitOfWork)
        {
            _transactions = transactions;
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Checking arguments.
                if (request.TransactionId == Guid.Empty)
                    return Result<Guid>.Fail("TransactionId is empty.", 400);

                // Find and check transaction data.
                var transactionFindResult = await _transactions.GetByExpressionAsync(a => a.Id == request.TransactionId);

                if (!transactionFindResult.Success || transactionFindResult.Value == null)
                    return Result<Guid>.Fail($"The transaction not found or already deleted.", 404);

                var transaction = transactionFindResult.Value;

                // Check and update sender data
                Customer? sender = null;

                var senderIdResult = await _customers.GetByIdAsync(transaction.SenderId);
                if (!senderIdResult.Success || senderIdResult.Value == null)
                    return Result<Guid>.Fail($"Sender customer not found or already deleted.", 404);

                sender = senderIdResult.Value;

                sender.Balance += transaction.TransferredAmount;

                var witdrawResult = await _customers.UpdateAsync(sender);
                if (!witdrawResult.Success)
                    return Result<Guid>.Fail("The sender's transferred amount has not been returned.", 500);

                // Check and update recipient data
                Customer? recipient = null;

                var recipientIdResult = await _customers.GetByIdAsync(transaction.RecipientId);
                if (!recipientIdResult.Success || recipientIdResult.Value == null)
                    return Result<Guid>.Fail("The recipient's transferred amount has not been returned.", 404);

                recipient = recipientIdResult.Value;

                recipient.Balance += transaction.TransferredAmount;

                var addBalanceResult = await _customers.UpdateAsync(recipient);
                if (!addBalanceResult.Success)
                    return Result<Guid>.Fail("Recipient balance not added.", 500);

                transaction.UpdateDate = DateTime.UtcNow;

                var updateStatus = await _transactions.UpdateAsync(transaction);
                if (!updateStatus.Success)
                    return Result<Guid>.Fail(updateStatus.Message, updateStatus.StatusCode);

                var savedChanges = await _unitOfWork.SaveChangesAsync();

                if (!savedChanges.Success)
                    return Result<Guid>.Fail(savedChanges.Message, savedChanges.StatusCode);

                await _unitOfWork.CommitTransactionAsync();
                return Result<Guid>.Succeed(request.TransactionId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<Guid>.Fail(ex.Message, 500);
            }
        }
    }
}
