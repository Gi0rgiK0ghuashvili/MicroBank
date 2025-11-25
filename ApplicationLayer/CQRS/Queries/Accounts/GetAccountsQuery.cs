using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Accounts
{
    public record GetAccountsQuery(bool Active = true) : IRequest<Result<IEnumerable<AccountDTO>>>;
    
    internal class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, Result<IEnumerable<AccountDTO>>>
    {
        private readonly IGenericRepository<Account> _accounts;

        public GetAccountsQueryHandler(IGenericRepository<Account> accounts)
        {
            _accounts = accounts;
        }

        public async Task<Result<IEnumerable<AccountDTO>>> Handle(GetAccountsQuery request, CancellationToken cancellation)
        {
            try
            {
                var accountsResult = await _accounts.ListAsync(a => a.Active);

                if (accountsResult == null || !accountsResult.Success || accountsResult.Value == null)
                    return Result<IEnumerable<AccountDTO>>.Fail("Accounts not found.", 404);
                

                if(!accountsResult.Value.Any())
                    return Result<IEnumerable<AccountDTO>>.Fail("Accounts data base is empty.", 404);


                var accounts = accountsResult.Value;
                var accountDtos = new List<AccountDTO>();

                foreach (var account in accounts) 
                {
                    accountDtos.Add(account.ToDTO());
                }
                return Result<IEnumerable<AccountDTO>>.Succeed(accountDtos);


            }
            catch (Exception ex)
            {

                return Result<IEnumerable<AccountDTO>>.Fail(ex.Message, 500);

            }



        }
    }
}
