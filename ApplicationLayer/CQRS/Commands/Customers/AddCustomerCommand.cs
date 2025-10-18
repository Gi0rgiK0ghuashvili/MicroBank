using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Customers
{
    public record AddCustomerCommand(Customer Customer) : IRequest<Result<int>>;


    internal class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, Result<int>>
    {
        private readonly IGenericRepository<Customer> _customers;
        private readonly IUnitOfWork _unitOfWork;

        public AddCustomerCommandHandler(IGenericRepository<Customer> customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerFind = await _customers.GetByExpressionAsync(a => a.Id == request.Customer.Id);

                if (!customerFind.Success)
                {
                    var addCustomerResult = await _customers.AddAsync(request.Customer);
                    if (addCustomerResult.Success)
                    {
                        var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                        if (saveChangesResult.Success)
                        {
                            return Result<int>.Succeed(request.Customer.Id);
                        }
                        else
                        {
                            return Result<int>.Fail($"დაფიქსირდა შეცდომა დამატებული მონაცემების შენახვისას. შეტყობინება: {customerFind.Message}", 401);
                        }
                    }
                    else
                    {
                        return Result<int>.Fail($"დაფიქსირდა შეცდომა მონაცემების დამატებისას. შეტყობინება: {customerFind.Message}", 401);
                    }
                }
                else
                {
                    return Result<int>.Fail($"მომხმარებელი უკვე დამატებულია. შეტყობინება: {customerFind.Message}", 401);
                }
            }
            catch (Exception ex)
            {
                return Result<int>.Fail(ex.Message, 500);
            }
        }
    }
}
