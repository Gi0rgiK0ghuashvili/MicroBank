using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Accounts
{
    public record GetAccountByIdQuery(Guid AccountId) : IRequest<Result<AccountDTO>>;

    internal class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Result<AccountDTO>>
    {
        private readonly IGenericRepository<Account> _accounts;

        public GetAccountByIdQueryHandler(IGenericRepository<Account> accounts)
        {
            _accounts = accounts;
        }

        public async Task<Result<AccountDTO>> Handle(GetAccountByIdQuery request, CancellationToken cancellation)
        {
            try
            {
                if (request.AccountId == Guid.Empty)
                    return Result<AccountDTO>.Fail("Account Id is empty.");

                var accounts = await _accounts.GetByIdAsync(request.AccountId);

                if (accounts == null || accounts.Value == default)
                    return Result<AccountDTO>.Fail("Account not found.", 404);

                var accountDto = accounts.Value.ToDTO();
                
                return Result<AccountDTO>.Succeed(accountDto);
            }
            catch (Exception ex)
            {
                return Result<AccountDTO>.Fail(ex.Message, 500);
            }
        }
    }
}
