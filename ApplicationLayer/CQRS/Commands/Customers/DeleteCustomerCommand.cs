using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Customers
{
    public record DeleteCustomerCommand(int Id) : IRequest<Result<int>>;

    internal class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<int>>
    {
        private readonly IGenericRepository<Customer> _customers;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerCommandHandler(IGenericRepository<Customer> customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerFind = await _customers.GetByExpressionAsync(a => a.Id == request.Id);

                if (customerFind.Success)
                {
                    var customer = customerFind.Value;

                    customer.UpdateDate = DateTime.UtcNow;
                    customer.Active = false;

                    var updateStatus = await _customers.UpdateAsync(customer);
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
                    return Result<int>.Fail($"ექაუნთი უკვე დამატებულია. შეტყობინება: {customerFind.Message}", 401);
                }
            }
            catch (Exception ex)
            {
                return Result<int>.Fail(ex.Message, 500);
            }
        }
    }
}
