using ApplicationLayer.Interfaces;
using DomainLayer;
using DomainLayer.EntityModels;
using MediatR;

namespace ApplicationLayer.CQRS.Commands.Customers
{
    public record AddCustomerCommand(string Name, string Surname, string Email, string CreatedBy = "", decimal Balance = 0, Guid? AccountId = default) : IRequest<Result<Guid>>;


    internal class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, Result<Guid>>
    {
        private readonly IGenericRepository<Customer> _customers;
        private readonly IUnitOfWork _unitOfWork;

        public AddCustomerCommandHandler(IGenericRepository<Customer> customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customerFind = await _customers.GetByExpressionAsync(a => a.Email == request.Email);

                if (customerFind.Success)
                    return Result<Guid>.Fail("Customer with this email already exists.", 400);

                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Active = true,
                    CreatedDate = DateTime.UtcNow,

                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email,
                    Balance = request.Balance,
                    AccountId = request.AccountId == Guid.Empty ? Guid.NewGuid() : request.AccountId
                };

                var addCustomerResult = await _customers.AddAsync(customer);
                if (!addCustomerResult.Success)
                    return Result<Guid>.Fail("An error occurred while adding the customer.", 500);


                var saveChangesResult = await _unitOfWork.SaveChangesAsync();
                if (!saveChangesResult.Success)
                    return Result<Guid>.Fail($"An error occurred while saving the added data.", 500);

                return Result<Guid>.Succeed(customer.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Fail(ex.Message, 500);
            }
        }
    }
}
