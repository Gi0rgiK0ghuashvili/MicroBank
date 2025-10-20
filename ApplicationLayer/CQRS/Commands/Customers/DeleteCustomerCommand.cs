using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Customers
{
    public record DeleteCustomerCommand(Guid Id) : IRequest<Result<Guid>>;

    internal class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Customer> _customers;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerCommandHandler(IGenericRepository<Customer> customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == Guid.Empty)
                    return Result<Guid>.Fail("Invalid Id provided.", 400);

                var customerFind = await _customers.GetByExpressionAsync(a => a.Id == request.Id);

                if (!customerFind.Success || customerFind.Value == null)
                    return Result<Guid>.Fail($"Customer not found or already deleted.", 404);


                var customer = customerFind.Value;

                if (!customer.Active)
                    return Result<Guid>.Fail("Customer is already deleted.", 400);

                customer.Active = false;
                customer.UpdateDate = DateTime.UtcNow;

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
