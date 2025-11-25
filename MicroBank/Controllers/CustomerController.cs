using ApplicationLayer.CQRS.Commands.Customers;
using ApplicationLayer.CQRS.Queries.Customers;
using MediatR;
using MicroBank.DTO_Models.Customers;
using Microsoft.AspNetCore.Mvc;

namespace MicroBank.Controllers
{
    [ApiController]
    [Route("api[controller]")]
    public class CustomerController : Controller
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var getCustomersQuery = new GetCustomersQuery();
            var customersResult = await _mediator.Send(getCustomersQuery);
            if (!customersResult.Success || customersResult.Value == default)
                return BadRequest(customersResult.Message);

            if (!customersResult.Value.Any())
                return NotFound(customersResult.Message);

            var customersDto = customersResult.Value;

            return Ok();
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetById([FromBody] Guid customerId)
        {
            if(customerId == Guid.Empty)
                return BadRequest();

            var getCustomersQuery = new GetCustomerByIdQuery(customerId);
            var customersResult = await _mediator.Send(getCustomersQuery);
            if (!customersResult.Success || customersResult.Value == default)
                return BadRequest(customersResult.Message);

            return Ok(customersResult.Value);
        }

        [HttpGet("getByUserName")]
        public async Task<IActionResult> GetByUserName([FromBody] string userName)
        {
            if(string.IsNullOrEmpty(userName))
                return BadRequest();

            var getcustomerQuery = new GetCustomerByUserNameQuery(userName);
            var getCustomerResult = await _mediator.Send(getcustomerQuery);
            if(!getCustomerResult.Success || getCustomerResult.Value == default)
                return BadRequest(getCustomerResult.Message);

            return Ok(getCustomerResult.Value);
        }

        [HttpPost("addCustomer")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerModel model)
        {
            var addCustomerCommand = new AddCustomerCommand(
                model.Name,
                model.Surname,
                model.Email,
                model.CreatedBy,
                model.Balance.Value,
                model.AccountId);
            var customerResult = await _mediator.Send(addCustomerCommand);

            if (!customerResult.Success || customerResult.Value == Guid.Empty)
                return BadRequest(customerResult.Message);

            return Ok(customerResult.Value);
        }

        [HttpPost("updateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerModel model)
        {
            if(model == null)
                return BadRequest();

            var updateCustomerCommand = new UpdateCustomerCommand(
                model.Id.Value,
                model.Balance.Value,
                model.Name,
                model.Surname,
                model.Email,
                model.UpdateBy);

            var updateCustomerResult = await _mediator.Send(updateCustomerCommand);
            if(!updateCustomerResult.Success || updateCustomerResult.Value == Guid.Empty)
                return BadRequest(updateCustomerResult.Message);

            return Ok(updateCustomerResult.Value);
        }

        [HttpPost("deleteCustomer")]
        public async Task<IActionResult> DeleteCustomer([FromBody] Guid id)
        {
            if(id == Guid.Empty) 
                return BadRequest();

            var deleteCustomerCommand = new DeleteCustomerCommand(id);
            var customerResult = await _mediator.Send(deleteCustomerCommand);
            if(!customerResult.Success || customerResult.Value == Guid.Empty)
                return BadRequest(customerResult.Message);

            return Ok(customerResult.Value);
        }
    }
}
