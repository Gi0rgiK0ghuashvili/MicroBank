using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Accounts
{
    public record DeleteAccountCommand(Guid Id) : IRequest<Result<Guid>>;


    internal class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Account> _accounts;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountCommandHandler(IGenericRepository<Account> accounts, IUnitOfWork unitOfWork)
        {
            _accounts = accounts;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == Guid.Empty)
                    return Result<Guid>.Fail("Invalid Id provided.", 400);

                var accountFind = await _accounts.GetByExpressionAsync(a => a.Id == request.Id);

                if (!accountFind.Success || accountFind.Value == null)
                    return Result<Guid>.Fail($"Account not found or already deleted.", 404);

                var account = accountFind.Value;
                
                if (!account.Active)
                    return Result<Guid>.Fail("Account is already deleted.", 400);

                account.Active = false;
                account.UpdateDate = DateTime.UtcNow;

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
