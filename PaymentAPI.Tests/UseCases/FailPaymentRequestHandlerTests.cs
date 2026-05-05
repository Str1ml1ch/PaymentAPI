using Moq;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Models;
using PaymentAPI.Domain.Exceptions;
using PaymentAPI.Domain.Services;
using PaymentAPI.Domain.Storage.GetPaymentById;
using PaymentAPI.Domain.Storage.UpdatePayment;
using PaymentAPI.Domain.UseCases.FailPayment;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.UseCases;

public class FailPaymentRequestHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesStatusAndCancelsOrder_WhenPaymentExists()
    {
        var getStorage = new Mock<IGetPaymentByIdStorage>();
        var updateStorage = new Mock<IUpdatePaymentStorage>();
        var orderClient = new Mock<IOrderApiClient>();
        var paymentId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        var model = new PaymentModel
        {
            Id = paymentId,
            OrderId = orderId,
            ExternalPaymentTranscationId = "txn",
            Amount = 50m,
            Currency = ECurrency.USD,
            Status = EPaymentStatus.Pending,
            PaymentProvider = EPaymentProvider.Stripe,
            CreatedAt = DateTimeOffset.UtcNow
        };

        getStorage.Setup(s => s.GetByIdAsync(paymentId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(model);

        var handler = new FailPaymentRequestHandler(getStorage.Object, updateStorage.Object, orderClient.Object);
        var request = new FailPaymentRequest { PaymentId = paymentId, UpdatedBy = updatedBy };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result);
        updateStorage.Verify(s => s.UpdateStatusAsync(paymentId, EPaymentStatus.Failed, updatedBy, It.IsAny<CancellationToken>()), Times.Once);
        orderClient.Verify(c => c.CancelOrderAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsPaymentNotFoundException_WhenNotFound()
    {
        var getStorage = new Mock<IGetPaymentByIdStorage>();
        getStorage.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((PaymentModel?)null);
        var handler = new FailPaymentRequestHandler(getStorage.Object, new Mock<IUpdatePaymentStorage>().Object, new Mock<IOrderApiClient>().Object);

        await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => handler.Handle(new FailPaymentRequest { PaymentId = Guid.NewGuid() }, CancellationToken.None));
    }
}
