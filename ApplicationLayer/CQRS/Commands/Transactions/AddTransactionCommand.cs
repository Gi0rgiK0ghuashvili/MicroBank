using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Transactions
{
    public record AddTransactionCommand(Guid SenderId, Guid RecipientId, decimal TransferredAmount, string? CreatedBy = " ")
        : IRequest<Result<Guid>>;
    internal class AddTransactionCommandHandler : IRequestHandler<AddTransactionCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Transaction> _transactions;
        private readonly IGenericRepository<Customer> _customers;
        private readonly IUnitOfWork _unitOfWork;

        public AddTransactionCommandHandler(IGenericRepository<Transaction> transactions, IUnitOfWork unitOfWork, IGenericRepository<Customer> customers)
        {
            _transactions = transactions;
            _unitOfWork = unitOfWork;
            _customers = customers;
        }


        public async Task<Result<Guid>> Handle(AddTransactionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Checking arguments.
                if (request.SenderId == Guid.Empty)
                    return Result<Guid>.Fail("SenderId is empty.", 400);

                if (request.RecipientId == Guid.Empty)
                    return Result<Guid>.Fail("RecipientId is empty.", 400);
                
                if (request.SenderId == request.RecipientId)
                    return Result<Guid>.Fail("Cannot transfer to the same account.", 400);

                if (request.TransferredAmount <= 0)
                    return Result<Guid>.Fail("The transferred amount is equaled to or less than zero", 400);

                // Checking data.
                var senderIdResult = await _customers.GetByIdAsync(request.SenderId);
                if (!senderIdResult.Success || senderIdResult.Value == null)
                    return Result<Guid>.Fail($"Sender customer not found or already deleted.", 404);

                var sender = senderIdResult.Value;
                if (sender.Balance < request.TransferredAmount)
                    return Result<Guid>.Fail("The transferred amount exceeds the user's balance.", 400);


                var recipientIdResult = await _customers.GetByIdAsync(request.RecipientId);
                if (!recipientIdResult.Success || recipientIdResult.Value == null)
                    return Result<Guid>.Fail("Recipient customer not found or already deleted.", 404);

                var recipient = recipientIdResult.Value;

                sender.Balance -= request.TransferredAmount;
                var witdrawResult = await _customers.UpdateAsync(sender);
                if (!witdrawResult.Success)
                    return Result<Guid>.Fail("Sender balance not withdrawed.", 500);

                recipient.Balance += request.TransferredAmount;
                var addBalanceResult = await _customers.UpdateAsync(recipient);
                if(!addBalanceResult.Success)
                    return Result<Guid>.Fail("Recipient balance not added.", 500);


                // Create transaction
                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    Active = true,

                    SenderId = sender.Id,
                    RecipientId = recipient.Id,
                    TransferredAmount = request.TransferredAmount
                };

                var addTransactionResult = await _transactions.AddAsync(transaction);
                if (!addTransactionResult.Success) 
                    return Result<Guid>.Fail("An error occurred while adding the transaction.", 400);

                var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                if (!saveChangesResult.Success)
                    return Result<Guid>.Fail("An error occurred while saving the added data.", 401);
                
                await _unitOfWork.CommitTransactionAsync();

                return Result<Guid>.Succeed(transaction.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<Guid>.Fail(ex.Message, 500);
            }
        }

    }
}
