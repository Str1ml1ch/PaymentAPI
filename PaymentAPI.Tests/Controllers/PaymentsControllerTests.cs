using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentAPI.Controllers;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Models;
using PaymentAPI.Domain.UseCases.CompletePayment;
using PaymentAPI.Domain.UseCases.CreatePayment;
using PaymentAPI.Domain.UseCases.FailPayment;
using PaymentAPI.Domain.UseCases.GetPayment;
using System.Security.Claims;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.Controllers;

public class PaymentsControllerTests
{
    private static PaymentsController CreateController(IMediator mediator, Guid? userId = null)
    {
        var controller = new PaymentsController(mediator);
        if (userId.HasValue)
        {
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.Value.ToString()) };
            var identity = new ClaimsIdentity(claims, "Test");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }
        else
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
        return controller;
    }

    [Fact]
    public async Task Get_ReturnsOk_WithPaymentModel()
    {
        var mediator = new Mock<IMediator>();
        var paymentId = Guid.NewGuid();
        var model = new PaymentModel
        {
            Id = paymentId,
            OrderId = Guid.NewGuid(),
            Status = EPaymentStatus.Pending,
            ExternalPaymentTranscationId = "txn",
            Amount = 100m,
            Currency = ECurrency.USD,
            PaymentProvider = EPaymentProvider.Stripe,
            CreatedAt = DateTimeOffset.UtcNow
        };
        mediator.Setup(m => m.Send(It.IsAny<GetPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(model);
        var controller = CreateController(mediator.Object, Guid.NewGuid());

        var result = await controller.Get(paymentId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(model, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsOk_WithPaymentId()
    {
        var mediator = new Mock<IMediator>();
        var paymentId = Guid.NewGuid();
        mediator.Setup(m => m.Send(It.IsAny<CreatePaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentId);
        var controller = CreateController(mediator.Object);
        var request = new CreatePaymentRequest { OrderId = Guid.NewGuid(), Amount = 50m, Currency = ECurrency.USD };

        var result = await controller.Create(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Complete_ReturnsNoContent()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<CompletePaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        var controller = CreateController(mediator.Object, Guid.NewGuid());
        var paymentId = Guid.NewGuid();

        var result = await controller.Complete(paymentId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Complete_SendsCorrectPaymentId()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<CompletePaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        var userId = Guid.NewGuid();
        var controller = CreateController(mediator.Object, userId);
        var paymentId = Guid.NewGuid();

        await controller.Complete(paymentId, CancellationToken.None);

        mediator.Verify(m => m.Send(
            It.Is<CompletePaymentRequest>(r => r.PaymentId == paymentId && r.UpdatedBy == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Failed_ReturnsNoContent()
    {
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<FailPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        var controller = CreateController(mediator.Object, Guid.NewGuid());

        var result = await controller.Failed(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}
