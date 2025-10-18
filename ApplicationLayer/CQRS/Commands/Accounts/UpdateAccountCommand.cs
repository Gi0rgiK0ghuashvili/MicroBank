using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Accounts
{
    public record UpdateAccountCommand(Account Account) : IRequest<Result<int>>;

    internal class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result<int>>
    {
        private readonly IGenericRepository<Account> _accounts;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAccountCommandHandler(IGenericRepository<Account> accounts, IUnitOfWork unitOfWork)
        {
            _accounts = accounts;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountFind = await _accounts.GetByExpressionAsync(a => a.Id == request.Account.Id);

                if (accountFind.Success)
                {
                    var account = accountFind.Value;

                    account.UpdateDate = DateTime.UtcNow;
                    account.Active = request.Account.Active;
                    account.UserName = request.Account.UserName;
                    account.CustomerId = request.Account.CustomerId;
                    account.TransactionId = request.Account.TransactionId;

                    account.PasswordHash = request.Account.PasswordHash;
                    account.PasswordSalt = request.Account.PasswordSalt;
                    
                    var updateStatus = await _accounts.UpdateAsync(account);
                    if (updateStatus.Success)
                    {
                        var savedChanges = await _unitOfWork.SaveChangesAsync();

                        if (savedChanges.Success)
                            return Result<int>.Succeed(request.Account.Id);
                        else
                            return Result<int>.Fail(savedChanges.Message, savedChanges.StatusCode);

                    }
                    else
                        return Result<int>.Fail(updateStatus.Message, updateStatus.StatusCode);
                }
                else
                {
                    return Result<int>.Fail($"ექაუნთი უკვე დამატებულია. შეტყობინება: {accountFind.Message}", 401);
                }
            }
            catch (Exception ex)
            {
                return Result<int>.Fail(ex.Message, 500);
            }
        }
    }
}
