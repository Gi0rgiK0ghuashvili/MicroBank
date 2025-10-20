using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Queries.Customers
{
    public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<Customer>>;

    internal class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<Customer>>
    {
        private readonly IGenericRepository<Customer> _customers;

        public GetCustomerByIdQueryHandler(IGenericRepository<Customer> customers)
        {
            _customers = customers;
        }

        public async Task<Result<Customer>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.CustomerId == Guid.Empty)
                    return Result<Customer>.Fail("Customer Id is empty.");

                var transaction = await _customers.GetByIdAsync(request.CustomerId);

                if (transaction == null)
                    return Result<Customer>.Fail("Customer not found.", 404);

                return transaction;
            }
            catch (Exception ex)
            {
                return Result<Customer>.Fail(ex.Message, 500);
            }
        }
    }
}
