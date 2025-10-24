using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Customers
{
    public record UpdateCustomerCommand(Guid Id, decimal Balance, string? Name = "", string? Surname = " ", string? Email = " ", string? UpdateBy = " ")
        : IRequest<Result<Guid>>;

    internal class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Customer> _customers;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(IGenericRepository<Customer> customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerFind = await _customers.GetByExpressionAsync(a => a.Id == request.Id);

                if (!customerFind.Success || customerFind.Value == null)
                    return Result<Guid>.Fail("Customer not found or already deleted.", 400);

                var customer = customerFind.Value;

                customer.Balance = request.Balance;
                customer.UpdateDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(request.Name))
                    customer.Name = request.Name;

                if (!string.IsNullOrEmpty(request.Surname))
                    customer.Surname = request.Surname;

                if (!string.IsNullOrEmpty(request.Email))
                    customer.Email = request.Email;

                if(!string.IsNullOrEmpty(request.UpdateBy))
                    customer.UpdateBy = request.UpdateBy;

                var updateStatus = await _customers.UpdateAsync(customer);
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
