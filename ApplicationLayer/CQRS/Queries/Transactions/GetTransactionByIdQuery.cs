using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Transactions
{
    public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<Result<Transaction>>;

    internal class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result<Transaction>>
    {
        private readonly IGenericRepository<Transaction> _transactions;

        public GetTransactionByIdQueryHandler(IGenericRepository<Transaction> transactions)
        {
            _transactions = transactions;
        }

        public async Task<Result<Transaction>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.TransactionId == Guid.Empty)
                    return Result<Transaction>.Fail("Transaction Id is empty.");

                var transaction = await _transactions.GetByIdAsync(request.TransactionId);

                if (transaction == null)
                    return Result<Transaction>.Fail("Transaction not found.", 404);

                return transaction;
            }
            catch (Exception ex)
            {
                return Result<Transaction>.Fail(ex.Message, 500);
            }
        }
    }
}
