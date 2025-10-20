using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MicroBank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("Get All")]
        public async Task<IActionResult> GetAll()
        {


            return Ok();
        }

        [HttpGet("Get By Id")]
        public async Task<IActionResult> GetById()
        {


            return Ok();
        }

        [HttpGet("Get By UserName")]
        public async Task<IActionResult> GetByUserName()
        {


            return Ok();
        }

        [HttpPost("Add Customer")]
        public async Task<IActionResult> AddCustomer()
        {


            return Ok();
        }

        [HttpPost("Update Customer")]
        public async Task<IActionResult> UpdateCustomer()
        {


            return Ok();
        }

        [HttpPost("Delete Customer")]
        public async Task<IActionResult> DeleteCustomer()
        {


            return Ok();
        }
    }
}
