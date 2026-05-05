using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Enums;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Entities;
using PaymentAPI.DAL.Storage.GetPaymentById;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.DAL;

public class GetPaymentByIdStorageTests
{
    private static PaymentDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PaymentDbContext(options);
    }

    private static Payment Seed(PaymentDbContext ctx,
        Guid? id = null, Guid? orderId = null, string txnId = "txn-1",
        EPaymentStatus status = EPaymentStatus.Pending)
    {
        var p = new Payment
        {
            Id = id ?? Guid.NewGuid(),
            OrderId = orderId ?? Guid.NewGuid(),
            ExternalPaymentTranscationId = txnId,
            Amount = 50m,
            Currency = ECurrency.USD,
            Status = status,
            PaymentProvider = EPaymentProvider.Stripe,
            CreatedAt = DateTimeOffset.UtcNow
        };
        ctx.Payments.Add(p);
        ctx.SaveChanges();
        return p;
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        using var ctx = CreateContext();
        var storage = new GetPaymentByIdStorage(ctx);

        var result = await storage.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsModel_WhenFound()
    {
        using var ctx = CreateContext();
        var payment = Seed(ctx);
        var storage = new GetPaymentByIdStorage(ctx);

        var result = await storage.GetByIdAsync(payment.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(payment.Id, result.Id);
        Assert.Equal(payment.OrderId, result.OrderId);
        Assert.Equal(payment.ExternalPaymentTranscationId, result.ExternalPaymentTranscationId);
        Assert.Equal(payment.Amount, result.Amount);
        Assert.Equal(payment.Status, result.Status);
    }

    [Fact]
    public async Task GetByOrderIdAsync_ReturnsModel_WhenFound()
    {
        using var ctx = CreateContext();
        var orderId = Guid.NewGuid();
        var payment = Seed(ctx, orderId: orderId);
        var storage = new GetPaymentByIdStorage(ctx);

        var result = await storage.GetByOrderIdAsync(orderId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.OrderId);
    }

    [Fact]
    public async Task GetByExternalTransactionIdAsync_ReturnsModel_WhenFound()
    {
        using var ctx = CreateContext();
        Seed(ctx, txnId: "my-txn-999");
        var storage = new GetPaymentByIdStorage(ctx);

        var result = await storage.GetByExternalTransactionIdAsync("my-txn-999", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("my-txn-999", result.ExternalPaymentTranscationId);
    }

    [Fact]
    public async Task IsExistsAsync_ReturnsFalse_WhenNotFound()
    {
        using var ctx = CreateContext();
        var storage = new GetPaymentByIdStorage(ctx);

        var result = await storage.IsExistsAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task IsExistsAsync_ReturnsTrue_WhenFound()
    {
        using var ctx = CreateContext();
        var payment = Seed(ctx);
        var storage = new GetPaymentByIdStorage(ctx);

        var result = await storage.IsExistsAsync(payment.Id, CancellationToken.None);

        Assert.True(result);
    }
}
