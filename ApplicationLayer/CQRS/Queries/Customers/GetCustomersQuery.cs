using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Customers
{

    public record GetCustomersQuery(bool Active = true) : IRequest<Result<IEnumerable<CustomerDTO>>>;

    internal class GetCustomerQueryHandler :IRequestHandler<GetCustomersQuery, Result<IEnumerable<CustomerDTO>>>
    {
        private readonly IGenericRepository<Customer> _customers;

        public GetCustomerQueryHandler(IGenericRepository<Customer> customers)
        {
            _customers = customers;
        }

        public async Task<Result<IEnumerable<CustomerDTO>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var customersResult = await _customers.ListAsync(a => a.Active);

                if (customersResult == null || !customersResult.Success || customersResult.Value == null)
                    return Result<IEnumerable<CustomerDTO>>.Fail("Customer not found", 404);
                
                if(!customersResult.Value.Any())
                    return Result<IEnumerable<CustomerDTO>>.Fail( "Customers data base is empty.", 404);


                var customers = customersResult.Value;

                var customersDto = new List<CustomerDTO>();

                foreach (var customer in customers)
                {
                    customersDto.Add(customer.ToDTO());
                }

                return Result<IEnumerable<CustomerDTO>>.Succeed(customersDto);
                
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerDTO>>.Fail(ex.Message, 500);
            }
        }
    }
}
