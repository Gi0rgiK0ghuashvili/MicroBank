using ApplicationLayer.CQRS.Commands.Transactions;
using ApplicationLayer.CQRS.Queries.Transactions;
using MediatR;
using MicroBank.DTO_Models.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace MicroBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var getTransactionsQuery = new GetTransactionsQuery();
            var transactionsResult = await _mediator.Send(getTransactionsQuery);

            if (!transactionsResult.Success || transactionsResult.Value == null)
                return BadRequest(transactionsResult.Message);

            if (!transactionsResult.Value.Any())
                return NotFound(transactionsResult.Message);

            var transactions = transactionsResult.Value.ToList();

            return Ok(transactions);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromBody] Guid id)
        {
            var getTransactionByIdQuery = new GetTransactionByIdQuery(id);
            var transactionResult = await _mediator.Send(getTransactionByIdQuery);

            if (!transactionResult.Success || transactionResult.Value == null || transactionResult.Value == default)
                return BadRequest(transactionResult.Message);

            var transactions = transactionResult.Value;

            return Ok(transactions);
        }

        [HttpGet("GetBySender")]
        public async Task<IActionResult> GetBySender([FromBody] Guid senderId)
        {
            var getTransactionBySenderIdQuery = new GetTransactionBySenderIdQuery(senderId);
            var transactionResult = await _mediator.Send(getTransactionBySenderIdQuery);

            if (!transactionResult.Success || transactionResult.Value == null || transactionResult.Value == default)
                return BadRequest(transactionResult.Message);

            var transactions = transactionResult.Value;

            return Ok(transactions);
        }

        [HttpGet("GetByRecipient")]
        public async Task<IActionResult> GetByRecipient([FromBody] Guid recipientId)
        {
            var getTransactionBySenderIdQuery = new GetTransactionBySenderIdQuery(recipientId);
            var transactionResult = await _mediator.Send(getTransactionBySenderIdQuery);

            if (!transactionResult.Success || transactionResult.Value == null || transactionResult.Value == default)
                return BadRequest(transactionResult.Message);

            var transactions = transactionResult.Value;

            return Ok(transactions);
        }

        [HttpPost("AddTransaction")]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionModel model)
        {
            if (model == null || model == default)
                return BadRequest();

            if (model.SenderId == null || model.SenderId == Guid.Empty)
                return BadRequest(nameof(model.SenderId));

            if (model.RecipientId == null || model.RecipientId == Guid.Empty)
                return BadRequest(nameof(model.RecipientId));

            if (model.TransferredAmount == null || model.TransferredAmount == default)
                return BadRequest(nameof(model.TransferredAmount));

            if (string.IsNullOrEmpty(model.CreatedBy))
                return BadRequest(nameof(model.CreatedBy));

            var addTransactionQuery = new AddTransactionCommand(
                model.SenderId.Value,
                model.RecipientId.Value,
                model.TransferredAmount.Value,
                model.CreatedBy
                );
            var transactionResult = await _mediator.Send(addTransactionQuery);

            if (!transactionResult.Success || transactionResult.Value == Guid.Empty)
                return BadRequest(transactionResult.Message);

            var transactionId = transactionResult.Value;

            return Ok(transactionId);
        }

        [HttpPost("UpdateTransaction")]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionModel model)
        {
            if (model.Id == null || model.Id.Value == Guid.Empty)
                return BadRequest(nameof(model.Id));

            if (model.SenderId == null || model.SenderId.Value == Guid.Empty)
                return BadRequest(nameof(model.SenderId));

            if (model.RecipientId == null || model.RecipientId.Value == Guid.Empty)
                return BadRequest(nameof(model.RecipientId));

            if (model.TransferredAmount == null || model.TransferredAmount <= 0)
                return BadRequest(nameof(model.TransferredAmount));

            if (string.IsNullOrWhiteSpace(model.UpdateBy))
                return BadRequest(nameof(model.UpdateBy));

            var deleteTransactionCommand = new UpdateTransactionCommand(
                model.Id.Value,
                model.TransferredAmount.Value,
                model.SenderId,
                model.RecipientId,
                model.UpdateBy);
            var transactionResult = await _mediator.Send(deleteTransactionCommand);

            if (!transactionResult.Success || transactionResult.Value == Guid.Empty)
                return BadRequest(transactionResult.Message);

            var transactionId = transactionResult.Value;

            return Ok(transactionId);
        }

        [HttpPost("DeleteTransaction")]
        public async Task<IActionResult> DeleteTransaction([FromBody] Guid id)
        {
            if (id == null || id == Guid.Empty)
                return BadRequest();

            var deleteTransactionCommand = new DeleteTransactionCommand(id);
            var transactionResult = await _mediator.Send(deleteTransactionCommand);

            if (!transactionResult.Success || transactionResult.Value == Guid.Empty)
                return BadRequest(transactionResult.Message);

            var transactionId = transactionResult.Value;

            return Ok(transactionId);
        }
    }
}
