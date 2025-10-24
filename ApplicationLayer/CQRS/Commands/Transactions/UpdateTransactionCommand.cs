using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Transactions
{
    public record UpdateTransactionCommand(Guid TransactionId, decimal TransferredAmount, Guid? SenderId = default, Guid? RecipientId = default, string? UpdateBy = " ") : IRequest<Result<Guid>>;

    internal class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Transaction> _transactions;
        private readonly IGenericRepository<Customer> _customers;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTransactionCommandHandler(IGenericRepository<Transaction> transactions, IUnitOfWork unitOfWork, IGenericRepository<Customer> customers)
        {
            _transactions = transactions;
            _unitOfWork = unitOfWork;
            _customers = customers;
        }

        public async Task<Result<Guid>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Checking arguments.
                if (request.TransactionId == Guid.Empty)
                    return Result<Guid>.Fail("TransactionId is empty.", 400);

                if (request.SenderId == null || request.SenderId == Guid.Empty)
                    return Result<Guid>.Fail("SenderId is empty.", 400);

                if (request.RecipientId == null || request.RecipientId == Guid.Empty)
                    return Result<Guid>.Fail("RecipientId is empty.", 400);

                if (request.SenderId == request.RecipientId)
                    return Result<Guid>.Fail("Cannot transfer to the same account.", 400);

                if (request.TransferredAmount <= 0)
                    return Result<Guid>.Fail("The transferred amount is equaled to or less than zero", 400);

                // Find and check transaction data.
                var transactionFindResult = await _transactions.GetByExpressionAsync(a => a.Id == request.TransactionId);

                if (!transactionFindResult.Success || transactionFindResult.Value == null)
                    return Result<Guid>.Fail($"The transaction not found or already deleted.", 404);
                var transaction = transactionFindResult.Value;
                // Save current transferred amount.
                var oldTransferredAmount = transaction.TransferredAmount;
                var differenceAmount = oldTransferredAmount - request.TransferredAmount;
                if (differenceAmount < 0)
                    differenceAmount *= (-1);

                // Check and update sender data
                Customer? sender = null;
                Guid senderId = transaction.SenderId;
                if (request.SenderId != null && request.SenderId != Guid.Empty)
                    senderId = request.SenderId.Value;

                var senderIdResult = await _customers.GetByIdAsync(senderId);
                if (!senderIdResult.Success || senderIdResult.Value == null)
                    return Result<Guid>.Fail($"Sender customer not found or already deleted.", 404);

                sender = senderIdResult.Value;
                if (sender.Balance < request.TransferredAmount)
                    return Result<Guid>.Fail("The transferred amount exceeds the user's balance.", 400);

                if (differenceAmount > 0)
                    sender.Balance -= differenceAmount;

                var witdrawResult = await _customers.UpdateAsync(sender);
                if (!witdrawResult.Success)
                    return Result<Guid>.Fail("Sender balance not withdrawed.", 500);

                // Check and update recipient data
                Customer? recipient = null;
                Guid recipientId = transaction.RecipientId;

                if (request.RecipientId != Guid.Empty)
                    recipientId = request.RecipientId.Value;

                var recipientIdResult = await _customers.GetByIdAsync(recipientId);
                if (!recipientIdResult.Success || recipientIdResult.Value == null)
                    return Result<Guid>.Fail("Recipient customer not found or already deleted.", 404);

                recipient = recipientIdResult.Value;

                if (differenceAmount > 0)
                    recipient.Balance += differenceAmount;
                var addBalanceResult = await _customers.UpdateAsync(recipient);
                if (!addBalanceResult.Success)
                    return Result<Guid>.Fail("Recipient balance not added.", 500);


                transaction.UpdateDate = DateTime.UtcNow;

                transaction.SenderId = request.SenderId.Value;
                transaction.RecipientId = request.RecipientId.Value;
                transaction.TransferredAmount = request.TransferredAmount;

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
