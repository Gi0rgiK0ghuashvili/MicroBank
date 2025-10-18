using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Accounts
{
    public record DeleteAccountCommand(int Id) : IRequest<Result<int>>;


    internal class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<int>>
    {
        private readonly IGenericRepository<Account> _accounts;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountCommandHandler(IGenericRepository<Account> accounts, IUnitOfWork unitOfWork)
        {
            _accounts = accounts;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountFind = await _accounts.GetByExpressionAsync(a => a.Id == request.Id);

                if (accountFind.Success)
                {
                    var account = accountFind.Value;

                    account.UpdateDate = DateTime.UtcNow;
                    account.Active = false;

                    var updateStatus = await _accounts.UpdateAsync(account);
                    if (updateStatus.Success)
                    {
                        var savedChanges = await _unitOfWork.SaveChangesAsync();

                        if (savedChanges.Success)
                            return Result<int>.Succeed(request.Id);
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
