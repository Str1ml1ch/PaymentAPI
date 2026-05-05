using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Storage.CreatePayment;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Storage.CreatePayment;

namespace PaymentAPI.Tests.DAL;

public class CreatePaymentStorageTests
{
    private static PaymentDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PaymentDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_PersistsPayment_AndReturnsNewId()
    {
        using var ctx = CreateContext();
        var storage = new CreatePaymentStorage(ctx);
        var orderId = Guid.NewGuid();

        var id = await storage.CreateAsync(
            orderId,
            "txn-001",
            99.99m,
            Homework.Ticketing.System.Shared.Enums.ECurrency.USD,
            EPaymentStatus.Pending,
            EPaymentProvider.Stripe,
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        var entity = ctx.Payments.Single(p => p.Id == id);
        Assert.Equal(orderId, entity.OrderId);
        Assert.Equal("txn-001", entity.ExternalPaymentTranscationId);
        Assert.Equal(99.99m, entity.Amount);
        Assert.Equal(EPaymentStatus.Pending, entity.Status);
        Assert.Equal(EPaymentProvider.Stripe, entity.PaymentProvider);
    }
}
