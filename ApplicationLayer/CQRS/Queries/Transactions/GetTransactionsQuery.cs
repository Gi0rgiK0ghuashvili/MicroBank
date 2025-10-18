using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Transactions
{
    public record GetTransactionsQuery : IRequest<Result<IEnumerable<Transaction>>>;


    internal class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, Result<IEnumerable<Transaction>>>
    {
        private readonly IGenericRepository<Transaction> _transactions;

        public GetTransactionsQueryHandler(IGenericRepository<Transaction> transactions)
        {
            _transactions = transactions;
        }

        public async Task<Result<IEnumerable<Transaction>>> Handle(GetTransactionsQuery request, CancellationToken cancellation)
        {

            try
            {
                var transactions = await _transactions.ListAsync(x => x.Active);

                if(transactions == null)
                {
                    return Result<IEnumerable<Transaction>>.Fail("მონაცემები ვერ მოიძებნა.", 404);
                }
                else
                {
                    return transactions;
                }
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Transaction>>.Fail(ex.Message, 500);
            }

        }


    }
}
