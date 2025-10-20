using ApplicationLayer.CQRS.Queries.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MicroBank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("Get All")]
        public async Task<IActionResult> GetAll()
        {
            var getAllAccountsQuery = new GetAccountsQuery();
            var accountsResult = await _mediator.Send(getAllAccountsQuery);
            
            if(!accountsResult.Success || accountsResult.Value == null || accountsResult.Value.Count() <= 0)
                return BadRequest();

            return Ok(accountsResult.Value);
        }

        [HttpGet("Get By Id")]
        public async Task<IActionResult> GetById(Guid AccountId)
        {


            return Ok();
        }

        [HttpGet("Get By UserName")]
        public async Task<IActionResult> GetByUserName()
        {


            return Ok();
        }

        [HttpPost("Add Account")]
        public async Task<IActionResult> AddAccount()
        {


            return Ok();
        }

        [HttpPost("Update Account")]
        public async Task<IActionResult> UpdateAccount()
        {


            return Ok();
        }

        [HttpPost("Delete Account")]
        public async Task<IActionResult> DeleteAccount()
        {


            return Ok();
        }
    }
}
