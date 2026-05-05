using Moq;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Models;
using PaymentAPI.Domain.Exceptions;
using PaymentAPI.Domain.Storage.GetPaymentById;
using PaymentAPI.Domain.UseCases.GetPayment;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.UseCases;

public class GetPaymentRequestHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPayment_WhenFound()
    {
        var storage = new Mock<IGetPaymentByIdStorage>();
        var paymentId = Guid.NewGuid();
        var model = new PaymentModel
        {
            Id = paymentId,
            OrderId = Guid.NewGuid(),
            ExternalPaymentTranscationId = "txn",
            Amount = 200m,
            Currency = ECurrency.USD,
            Status = EPaymentStatus.Completed,
            PaymentProvider = EPaymentProvider.Stripe,
            CreatedAt = DateTimeOffset.UtcNow
        };
        storage.Setup(s => s.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(model);

        var handler = new GetPaymentRequestHandler(storage.Object);

        var result = await handler.Handle(new GetPaymentRequest { PaymentId = paymentId }, CancellationToken.None);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task Handle_ThrowsPaymentNotFoundException_WhenNotFound()
    {
        var storage = new Mock<IGetPaymentByIdStorage>();
        storage.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((PaymentModel?)null);

        var handler = new GetPaymentRequestHandler(storage.Object);

        await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => handler.Handle(new GetPaymentRequest { PaymentId = Guid.NewGuid() }, CancellationToken.None));
    }
}
