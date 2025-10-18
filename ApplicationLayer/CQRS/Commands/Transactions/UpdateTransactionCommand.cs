using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Transactions
{
    public record UpdateTransactionCommand(Transaction Transaction) : IRequest<Result<int>>;
    internal class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Result<int>>
    {
        private readonly IGenericRepository<Transaction> _transactions;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTransactionCommandHandler(IGenericRepository<Transaction> transactions, IUnitOfWork unitOfWork)
        {
            _transactions = transactions;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerFind = await _transactions.GetByExpressionAsync(a => a.Id == request.Transaction.Id);

                if (customerFind.Success)
                {
                    var customer = customerFind.Value;

                    customer.UpdateDate = DateTime.UtcNow;
                    customer.Active = request.Transaction.Active;

                    customer.SenderId = request.Transaction.SenderId;
                    customer.RecipientId = request.Transaction.RecipientId;
                    customer.TransferredAmount = request.Transaction.TransferredAmount;

                    var updateStatus = await _transactions.UpdateAsync(customer);
                    if (updateStatus.Success)
                    {
                        var savedChanges = await _unitOfWork.SaveChangesAsync();

                        if (savedChanges.Success)
                            return Result<int>.Succeed(request.Transaction.Id);
                        else
                            return Result<int>.Fail(savedChanges.Message, savedChanges.StatusCode);
                    }
                    else
                        return Result<int>.Fail(updateStatus.Message, updateStatus.StatusCode);
                }
                else
                {
                    return Result<int>.Fail($"ექაუნთი უკვე დამატებულია. შეტყობინება: {customerFind.Message}", 401);
                }
            }
            catch (Exception ex)
            {
                return Result<int>.Fail(ex.Message, 500);
            }
        }
    }
}
