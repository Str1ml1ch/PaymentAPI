using Moq;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Models;
using PaymentAPI.Domain.Exceptions;
using PaymentAPI.Domain.Storage.GetPaymentById;
using PaymentAPI.Domain.Storage.UpdatePayment;
using PaymentAPI.Domain.UseCases.CompletePayment;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.UseCases;

public class CompletePaymentRequestHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesStatus_WhenPaymentExists()
    {
        var getStorage = new Mock<IGetPaymentByIdStorage>();
        var updateStorage = new Mock<IUpdatePaymentStorage>();
        var paymentId = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();

        getStorage.Setup(s => s.IsExistsAsync(paymentId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

        var handler = new CompletePaymentRequestHandler(getStorage.Object, updateStorage.Object);
        var request = new CompletePaymentRequest { PaymentId = paymentId, UpdatedBy = updatedBy };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.True(result);
        updateStorage.Verify(s => s.UpdateStatusAsync(paymentId, EPaymentStatus.Completed, updatedBy, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsPaymentNotFoundException_WhenNotFound()
    {
        var getStorage = new Mock<IGetPaymentByIdStorage>();
        getStorage.Setup(s => s.IsExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(false);
        var updateStorage = new Mock<IUpdatePaymentStorage>();

        var handler = new CompletePaymentRequestHandler(getStorage.Object, updateStorage.Object);

        await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => handler.Handle(new CompletePaymentRequest { PaymentId = Guid.NewGuid() }, CancellationToken.None));
    }
}
