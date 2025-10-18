using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Accounts
{
    public record AddAccountCommand(Account Account) : IRequest<Result<int>>;

    internal class AddAccountCommandHandler : IRequestHandler<AddAccountCommand, Result<int>>
    {
        private readonly IGenericRepository<Account> _accounts;
        private readonly IUnitOfWork _unitOfWork;

        public AddAccountCommandHandler(IGenericRepository<Account> accounts, IUnitOfWork unitOfWork)
        {
            _accounts = accounts;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(AddAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountFind = await _accounts.GetByExpressionAsync(a => a.Id == request.Account.Id);

                if (!accountFind.Success) 
                {
                    var addAccountResult = await _accounts.AddAsync(request.Account);
                    if (addAccountResult.Success)
                    {
                        var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                        if (saveChangesResult.Success) 
                        {
                            return Result<int>.Succeed(request.Account.Id);
                        }
                        else
                        {
                            return Result<int>.Fail($"დაფიქსირდა შეცდომა დამატებული მონაცემების შენახვისას. შეტყობინება: {accountFind.Message}", 401);
                        }
                    }
                    else
                    {
                        return Result<int>.Fail($"დაფიქსირდა შეცდომა მონაცემების დამატებისას. შეტყობინება: {accountFind.Message}", 401);
                    }
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
