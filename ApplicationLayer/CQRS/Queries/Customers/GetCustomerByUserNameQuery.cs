using ApplicationLayer.CQRS.Converters;
using ApplicationLayer.CQRS.DTOs;
using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.CQRS.Queries.Customers
{
    public record GetCustomerByUserNameQuery(string UserName, bool Active = true) : IRequest<Result<CustomerDTO>>;

    internal class GetCustomerByUserNameQueryHandler : IRequestHandler<GetCustomerByUserNameQuery, Result<CustomerDTO>>
    {
        private readonly IGenericRepository<Customer> _customers;

        public GetCustomerByUserNameQueryHandler(IGenericRepository<Customer> customers)
        {
            _customers = customers;
        }

        public async Task<Result<CustomerDTO>> Handle(GetCustomerByUserNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserName))
                    return Result<CustomerDTO>.Fail("Argument is null.", 400);

                var customerResult = await _customers.GetByExpressionAsync(a => a.Account.UserName == request.UserName && a.Active == request.Active, "Account");

                if (customerResult == null || !customerResult.Success || customerResult.Value == default)
                    return Result<CustomerDTO>.Fail("Customer not found.", 404);


                var customer = customerResult.Value;

                var customerDto = customer.ToDTO();

                return Result<CustomerDTO>.Succeed(customerDto);
            }
            catch (Exception ex)
            {

                return Result<CustomerDTO>.Fail(ex.Message, 500);
            }
        }
    }
}
