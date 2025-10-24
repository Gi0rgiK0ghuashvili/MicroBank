using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Transactions
{
    public record GetTransactionsQuery : IRequest<Result<IEnumerable<TransactionDTO>>>;


    internal class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, Result<IEnumerable<TransactionDTO>>>
    {
        private readonly IGenericRepository<Transaction> _transactions;

        public GetTransactionsQueryHandler(IGenericRepository<Transaction> transactions)
        {
            _transactions = transactions;
        }

        public async Task<Result<IEnumerable<TransactionDTO>>> Handle(GetTransactionsQuery request, CancellationToken cancellation)
        {

            try
            {
                var transactionsResult = await _transactions.ListAsync(x => x.Active);

                if (transactionsResult == null || !transactionsResult.Success || transactionsResult.Value == default)
                    return Result<IEnumerable<TransactionDTO>>.Fail("Transactions not found.",404);

                if(!transactionsResult.Value.Any())
                    return Result<IEnumerable<TransactionDTO>>.Fail("Transactions data base is empty.", 404);


                var transactions = transactionsResult.Value;

                var transactionsDto = new List<TransactionDTO>();

                foreach (var transaction in transactions)
                {
                    transactionsDto.Add(transaction.ToDTO());
                }

                return Result<IEnumerable<TransactionDTO>>.Succeed(transactionsDto);

            }
            catch (Exception ex)
            {
                return Result<IEnumerable<TransactionDTO>>.Fail(ex.Message, 500);
            }

        }


    }
}
