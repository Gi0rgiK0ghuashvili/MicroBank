using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.CQRS.Commands.Accounts
{
    public record AddAccountCommand(string UserName, string Password, string CreatedBy = " ") : IRequest<Result<Guid>>;

    
    internal class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Account> _accounts;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHandler _passwordHandler;

        public AddAccountCommandHandler(IGenericRepository<Account> accounts, IUnitOfWork unitOfWork, IPasswordHandler passwordHandler)
        {
            _accounts = accounts;
            _unitOfWork = unitOfWork;
            _passwordHandler = passwordHandler;
        }

        public async Task<Result<Guid>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountFind = await _accounts.GetByExpressionAsync(a => a.UserName == request.UserName);

                if (accountFind.Success)
                    return Result<Guid>.Fail("Account with this username already exists.", 400);

                _passwordHandler.CreateSaltAndHash(request.Password, out byte[] hash, out byte[] salt);

                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName,
                    Active = true,

                    CreatedBy = request.CreatedBy,
                    CreatedDate = DateTime.UtcNow,

                    PasswordHash = hash,
                    PasswordSalt = salt,
                    CustomerId = null
                };
                
                var addAccountResult = await _accounts.AddAsync(account);

                if (!addAccountResult.Success)
                    return Result<Guid>.Fail("An error occurred while adding the account.", 500);
                var saveChangesResult = await _unitOfWork.SaveChangesAsync();

                if (!saveChangesResult.Success)
                    return Result<Guid>.Fail("An error occurred while saving the added data.", 500);
                 
                return Result<Guid>.Succeed(account.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Fail(ex.Message, 500);
            }
        }
    }
}
