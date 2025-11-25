using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Transactions
{
    public record GetTransactionByRecipientIdQuery(Guid RecipientId) : IRequest<Result<TransactionDTO>>;
    
    internal class GetTransactionByRecipientIdQueryHandler : IRequestHandler<GetTransactionByRecipientIdQuery, Result<TransactionDTO>>
    {

        private readonly IGenericRepository<Transaction> _transactions;

        public GetTransactionByRecipientIdQueryHandler(IGenericRepository<Transaction> transactions)
        {
            _transactions = transactions;
        }

        public async Task<Result<TransactionDTO>> Handle(GetTransactionByRecipientIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if(request == null)
                    return Result<TransactionDTO>.Fail("Argument is null or empty.");


                if (request.RecipientId == null || request.RecipientId == Guid.Empty)
                    return Result<TransactionDTO>.Fail("Transaction Id is empty.");

                var transactionResult = await _transactions.GetByExpressionAsync(t => t.RecipientId == request.RecipientId);

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
