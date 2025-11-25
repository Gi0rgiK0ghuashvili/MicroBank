using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Accounts
{
    public record UpdateAccountCommand(Guid Id, string UserName, string? Password = "", Guid? CustomerId = default, string? UpdateBy = "") 
        : IRequest<Result<Guid>>;

    internal class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Account> _accounts;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IPasswordHandler _passwordHandler;

        public UpdateAccountCommandHandler(IGenericRepository<Account> accounts, IUnitOfWork unitOfWork, IPasswordHandler passwordHandler)
        {
            _accounts = accounts;
            _unitOfWork = unitOfWork;
            _passwordHandler = passwordHandler;
        }

        public async Task<Result<Guid>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountFind = await _accounts.GetByIdAsync(request.Id);
                if (!accountFind.Success || accountFind.Value == null)
                    return Result<Guid>.Fail("Account not found or already deleted.", 400);

                var checkUserNameResult = await _accounts.GetByExpressionAsync(a => a.UserName == request.UserName);
                if(checkUserNameResult.Success)
                    return Result<Guid>.Fail("Account with this username already exists.", 400);

                var account = accountFind.Value;

                account.UserName = request.UserName;
                
                if(!string.IsNullOrEmpty(request.UpdateBy))
                    account.UpdateBy = request.UpdateBy;

                account.UpdateDate = DateTime.UtcNow;

                if(request.CustomerId != null && request.CustomerId.Value != Guid.Empty)
                    account.CustomerId = request.CustomerId.Value;

                if (!string.IsNullOrEmpty(request.Password))
                {
                    _passwordHandler.CreateSaltAndHash(request.Password, out byte[] hash, out byte[] salt);
                    account.PasswordHash = hash;
                    account.PasswordSalt = salt;
                }

                var updateStatus = await _accounts.UpdateAsync(account);
                if (!updateStatus.Success)
                    return Result<Guid>.Fail(updateStatus.Message, updateStatus.StatusCode);
                
                var savedChanges = await _unitOfWork.SaveChangesAsync();
                if (!savedChanges.Success)
                    return Result<Guid>.Fail(savedChanges.Message, savedChanges.StatusCode);
                
                return Result<Guid>.Succeed(request.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Fail(ex.Message, 500);
            }
        }
    }
}
