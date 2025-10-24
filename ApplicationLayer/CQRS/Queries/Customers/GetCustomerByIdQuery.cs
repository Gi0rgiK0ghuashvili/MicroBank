using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Customers
{
    public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<CustomerDTO>>;

    internal class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDTO>>
    {
        private readonly IGenericRepository<Customer> _customers;

        public GetCustomerByIdQueryHandler(IGenericRepository<Customer> customers)
        {
            _customers = customers;
        }

        public async Task<Result<CustomerDTO>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.CustomerId == Guid.Empty)
                    return Result<CustomerDTO>.Fail("Customer Id is empty.");

                var customerResult = await _customers.GetByIdAsync(request.CustomerId);

                if (customerResult == null || customerResult.Value == default)
                    return Result<CustomerDTO>.Fail("Customer not found.", 404);

                var customerDto = customerResult.Value.ToDTO(); ;


                return Result<CustomerDTO>.Succeed(customerDto);
            }
            catch (Exception ex)
            {
                return Result<CustomerDTO>.Fail(ex.Message, 500);
            }
        }
    }
}
