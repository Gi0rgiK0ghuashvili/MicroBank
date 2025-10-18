using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Accounts
{
    public record GetAccountsQuery(bool Active = true) : IRequest<Result<IEnumerable<Account>>>;
    
    internal class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, Result<IEnumerable<Account>>>
    {
        private readonly IGenericRepository<Account> _accounts;


        public async Task<Result<IEnumerable<Account>>> Handle(GetAccountsQuery request, CancellationToken cancellation)
        {
            try
            {
                var accounts = await _accounts.ListAsync(a => a.Active);

                if (accounts == null)
                {
                    return Result<IEnumerable<Account>>.Fail("მონაცემები ვერ მოიძებნა.", 404);
                }
                else
                {
                    return accounts;
                }
            }
            catch (Exception ex)
            {

                return Result<IEnumerable<Account>>.Fail(ex.Message, 500);

            }



        }
    }
}
