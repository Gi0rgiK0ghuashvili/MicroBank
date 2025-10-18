using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Customers
{

    public record GetCustomersQuery(bool Active = true) : IRequest<Result<IEnumerable<Customer>>>;

    internal class GetCustomerQueryHandler :IRequestHandler<GetCustomersQuery, Result<IEnumerable<Customer>>>
    {
        private readonly IGenericRepository<Customer> _customers;

        public GetCustomerQueryHandler(IGenericRepository<Customer> customers)
        {
            _customers = customers;
        }

        public async Task<Result<IEnumerable<Customer>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var customers = await _customers.ListAsync(a => a.Active);

                if (customers == null)
                {
                    return Result<IEnumerable<Customer>>.Fail("მონაცემები ვერ მოიძებნა.", 404);
                }
                else
                {
                    return customers;
                }
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Customer>>.Fail(ex.Message, 500);
            }
        }
    }
}
