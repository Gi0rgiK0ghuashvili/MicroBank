using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Transactions
{
    public record GetTransactionBySenderIdQuery(Guid SenderId) : IRequest<Result<TransactionDTO>>;

    internal class GetTransactionBySenderIdQueryHandler : IRequestHandler<GetTransactionBySenderIdQuery, Result<TransactionDTO>>
    {
        private readonly IGenericRepository<Transaction> _transactions;

        public GetTransactionBySenderIdQueryHandler(IGenericRepository<Transaction> transactions)
        {
            _transactions = transactions;
        }

        public async Task<Result<TransactionDTO>> Handle(GetTransactionBySenderIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.SenderId == null || request.SenderId == Guid.Empty)
                    return Result<TransactionDTO>.Fail("Transaction Id is empty.");

                var transactionResult = await _transactions.GetByExpressionAsync(t => t.SenderId == request.SenderId);

                if (transactionResult == null || transactionResult.Value == default)
                    return Result<TransactionDTO>.Fail("Transaction not found.", 404);

                var transactionDto = transactionResult.Value.ToDTO();

                return Result<TransactionDTO>.Succeed(transactionDto);
            }
            catch (Exception ex)
            {
                return Result<TransactionDTO>.Fail(ex.Message, 500);
            }
        }
    }
}
