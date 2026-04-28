using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Domain.UseCases.CompletePayment;
using PaymentAPI.Domain.UseCases.CreatePayment;
using PaymentAPI.Domain.UseCases.FailPayment;
using PaymentAPI.Domain.UseCases.GetPayment;
using System.Security.Claims;

namespace PaymentAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{payment_id}")]
        public async Task<IActionResult> Get(Guid payment_id, CancellationToken cancellationToken)
        {
            var payment = await _mediator.Send(new GetPaymentRequest { PaymentId = payment_id }, cancellationToken);
            return Ok(payment);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePaymentRequest request,
            CancellationToken cancellationToken)
        {
            var paymentId = await _mediator.Send(request, cancellationToken);
            return Ok(new { paymentId });
        }

        [HttpPost("{payment_id}/complete")]
        public async Task<IActionResult> Complete(Guid payment_id, CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdStr, out var userId);

            await _mediator.Send(new CompletePaymentRequest
            {
                PaymentId = payment_id,
                UpdatedBy = userId
            }, cancellationToken);
            return NoContent();
        }

        [HttpPost("{payment_id}/failed")]
        public async Task<IActionResult> Failed(Guid payment_id, CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdStr, out var userId);

            await _mediator.Send(new FailPaymentRequest
            {
                PaymentId = payment_id,
                UpdatedBy = userId
            }, cancellationToken);
            return NoContent();
        }
    }
}
