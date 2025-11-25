using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Accounts
{
    public record GetAccountByUserNameQuery(string UserName, bool Active = true) : IRequest<Result<AccountDTO>>;


    internal class GetAccountByUserNameQueryHandler : IRequestHandler<GetAccountByUserNameQuery, Result<AccountDTO>>
    {
        private readonly IGenericRepository<Account> _accounts;

        public GetAccountByUserNameQueryHandler(IGenericRepository<Account> accounts)
        {
            _accounts = accounts;
        }

        public async Task<Result<AccountDTO>> Handle(GetAccountByUserNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserName))
                    return Result<AccountDTO>.Fail("Argument is null.", 400);

                var accountResult = await _accounts.GetByExpressionAsync(a => a.UserName == request.UserName && a.Active == request.Active);
                
                if(accountResult == null || !accountResult.Success || accountResult.Value == default)
                    return Result<AccountDTO>.Fail("Account not found.", 404);


                var account = accountResult.Value;

                var accountDto = account.ToDTO();

                return Result<AccountDTO>.Succeed(accountDto);
            }
            catch (Exception ex)
            {

                return Result<AccountDTO>.Fail(ex.Message, 500);
            }
        }
    }
}
