using Moq;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Models;
using PaymentAPI.Domain.Storage.CreatePayment;
using PaymentAPI.Domain.UseCases.CreatePayment;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.UseCases;

public class CreatePaymentRequestHandlerTests
{
    [Fact]
    public async Task Handle_CallsStorageAndReturnsId()
    {
        var storage = new Mock<ICreatePaymentStorage>();
        var expectedId = Guid.NewGuid();
        storage.Setup(s => s.CreateAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<ECurrency>(), EPaymentStatus.Pending, EPaymentProvider.Stripe,
                It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedId);

        var handler = new CreatePaymentRequestHandler(storage.Object);
        var request = new CreatePaymentRequest { OrderId = Guid.NewGuid(), Amount = 80m, Currency = ECurrency.EUR };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(expectedId, result);
        storage.Verify(s => s.CreateAsync(
            request.OrderId, It.IsAny<string>(), request.Amount,
            request.Currency, EPaymentStatus.Pending, EPaymentProvider.Stripe,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
