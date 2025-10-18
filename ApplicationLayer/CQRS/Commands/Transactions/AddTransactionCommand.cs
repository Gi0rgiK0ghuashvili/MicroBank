using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Transactions
{
    public record AddTransactionCommand(Transaction Transaction) : IRequest<Result<int>>;
    internal class AddTransactionCommandHandler : IRequestHandler<AddTransactionCommand, Result<int>>
    {
        private readonly IGenericRepository<Transaction> _transactions;
        private readonly IUnitOfWork _unitOfWork;

        public AddTransactionCommandHandler(IGenericRepository<Transaction> transactions, IUnitOfWork unitOfWork)
        {
            _transactions = transactions;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<int>> Handle(AddTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transactionFind = await _transactions.GetByExpressionAsync(a => a.Id == request.Transaction.Id);

                if (!transactionFind.Success)
                {
                    var addTransactionResult = await _transactions.AddAsync(request.Transaction);
                    if (addTransactionResult.Success)
                    {
                        var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                        if (saveChangesResult.Success)
                        {
                            return Result<int>.Succeed(request.Transaction.Id);
                        }
                        else
                        {
                            return Result<int>.Fail($"დაფიქსირდა შეცდომა დამატებული მონაცემების შენახვისას. შეტყობინება: {transactionFind.Message}", 401);
                        }
                    }
                    else
                    {
                        return Result<int>.Fail($"დაფიქსირდა შეცდომა მონაცემების დამატებისას. შეტყობინება: {transactionFind.Message}", 401);
                    }
                }
                else
                {
                    return Result<int>.Fail($"მომხმარებელი უკვე დამატებულია. შეტყობინება: {transactionFind.Message}", 401);
                }
            }
            catch (Exception ex)
            {
                return Result<int>.Fail(ex.Message, 500);
            }
        }

    }
}
