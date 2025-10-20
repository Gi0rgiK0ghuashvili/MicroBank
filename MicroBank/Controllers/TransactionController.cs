using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MicroBank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
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

        [HttpGet("Get By Sender")]
        public async Task<IActionResult> GetBySender()
        {


            return Ok();
        }

        [HttpGet("Get By Recipient")]
        public async Task<IActionResult> GetByRecipient()
        {


            return Ok();
        }

        [HttpPost("Add Transaction")]
        public async Task<IActionResult> AddTransaction()
        {


            return Ok();
        }

        [HttpPost("Update Transaction")]
        public async Task<IActionResult> UpdateTransaction()
        {


            return Ok();
        }

        [HttpPost("Delete Transaction")]
        public async Task<IActionResult> DeleteTransaction()
        {


            return Ok();
        }
    }
}
