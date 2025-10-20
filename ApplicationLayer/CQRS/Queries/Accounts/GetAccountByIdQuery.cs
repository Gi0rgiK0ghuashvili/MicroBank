using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Accounts
{
    public record GetAccountByIdQuery(Guid AccountId) : IRequest<Result<Account>>;

    internal class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Result<Account>>
    {
        private readonly IGenericRepository<Account> _accounts;

        public GetAccountByIdQueryHandler(IGenericRepository<Account> accounts)
        {
            _accounts = accounts;
        }

        public async Task<Result<Account>> Handle(GetAccountByIdQuery request, CancellationToken cancellation)
        {
            try
            {
                if (request.AccountId == Guid.Empty)
                    return Result<Account>.Fail("Account Id is empty.");

                var accounts = await _accounts.GetByIdAsync(request.AccountId);

                if (accounts == null)
                    return Result<Account>.Fail("Account not found.", 404);

                return accounts;
            }
            catch (Exception ex)
            {
                return Result<Account>.Fail(ex.Message, 500);
            }
        }
    }
}
