using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Transactions
{
    public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<Result<TransactionDTO>>;

    internal class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result<TransactionDTO>>
    {
        private readonly IGenericRepository<Transaction> _transactions;

        public GetTransactionByIdQueryHandler(IGenericRepository<Transaction> transactions)
        {
            _transactions = transactions;
        }

        public async Task<Result<TransactionDTO>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.TransactionId == Guid.Empty)
                    return Result<TransactionDTO>.Fail("Transaction Id is empty.");

                var transactionResult = await _transactions.GetByIdAsync(request.TransactionId);

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
