using ApplicationLayer.CQRS.Commands.Accounts;
using ApplicationLayer.CQRS.Queries.Accounts;
using MediatR;
using MicroBank.DTO_Models.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace MicroBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get endpoints
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var getAllAccountsQuery = new GetAccountsQuery();
            var accountsResult = await _mediator.Send(getAllAccountsQuery);

            if (!accountsResult.Success)
                return BadRequest(accountsResult.Message);

            if (accountsResult.Value == null)
                return BadRequest(accountsResult.Message);

            if (!accountsResult.Value.Any())
                return NotFound(accountsResult.Message);

            return Ok(accountsResult.Value);
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetById(Guid accountId)
        {
            var getAllAccountsQuery = new GetAccountByIdQuery(accountId);
            var accountsResult = await _mediator.Send(getAllAccountsQuery);

            if (!accountsResult.Success || accountsResult.Value == null || accountsResult.Value == default)
                return BadRequest(accountsResult.Message);

            return Ok(accountsResult.Value);
        }

        [HttpGet("getByUserName")]
        public async Task<IActionResult> GetByUserName(string userName)
        {

            var getAllAccountsQuery = new GetAccountByUserNameQuery(userName);
            var accountsResult = await _mediator.Send(getAllAccountsQuery);

            if (!accountsResult.Success || accountsResult.Value == null || accountsResult.Value == default)
                return BadRequest(accountsResult.Message);

            return Ok(accountsResult.Value);
        }

        [HttpPost("addAccount")]
        public async Task<IActionResult> AddAccount([FromBody] AccountModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
                return BadRequest("UserName is null.");

            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Password is null.");

            var getAllAccountsQuery = new AddAccountCommand(model.UserName, model.Password, model.CreatedBy);
            var accountsResult = await _mediator.Send(getAllAccountsQuery);

            if (!accountsResult.Success || accountsResult.Value == Guid.Empty)
                return BadRequest(accountsResult.Message);

            return Ok(accountsResult.Value);
        }

        [HttpPost("updateAccount")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountModel model)
        {
            var updateAccount = new UpdateAccountCommand(
                model.Id.Value,
                model.UserName,
                model.Password,
                model.CustomerId,
                model.UpdateBy);
            var updateAccountResult = await _mediator.Send(updateAccount);

            if (!updateAccountResult.Success || updateAccountResult.Value == Guid.Empty)
                return BadRequest(updateAccountResult.Message);

            return Ok(updateAccountResult.Value);
        }

        [HttpPost("deleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] Guid accountId)
        {
            if (accountId == Guid.Empty)
                return BadRequest();

            var deleteAccountCommand = new DeleteAccountCommand(accountId);
            var deleteAccountResult = await _mediator.Send(deleteAccountCommand);

            if (!deleteAccountResult.Success || deleteAccountResult.Value == Guid.Empty)
                return BadRequest(deleteAccountResult.Message);

            return Ok(deleteAccountResult.Value);
        }
    }
}
