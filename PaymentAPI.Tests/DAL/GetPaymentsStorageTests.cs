using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Enums;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Entities;
using PaymentAPI.DAL.Storage.GetPayments;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.DAL;

public class GetPaymentsStorageTests
{
    private static PaymentDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PaymentDbContext(options);
    }

    private static void SeedMany(PaymentDbContext ctx, IEnumerable<Payment> payments)
    {
        ctx.Payments.AddRange(payments);
        ctx.SaveChanges();
    }

    private static Payment MakePayment(EPaymentStatus status = EPaymentStatus.Pending,
        EPaymentProvider provider = EPaymentProvider.Stripe,
        DateTimeOffset? createdAt = null)
        => new()
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            ExternalPaymentTranscationId = Guid.NewGuid().ToString(),
            Amount = 100m,
            Currency = ECurrency.USD,
            Status = status,
            PaymentProvider = provider,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow
        };

    [Fact]
    public async Task GetAsync_ReturnsAll_WhenNoFilters()
    {
        using var ctx = CreateContext();
        SeedMany(ctx, [MakePayment(), MakePayment(), MakePayment()]);
        var storage = new GetPaymentsStorage(ctx);

        var result = await storage.GetAsync(1, 10, null, null, null, null, CancellationToken.None);

        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Data!.Count);
    }

    [Fact]
    public async Task GetAsync_FiltersByStatus()
    {
        using var ctx = CreateContext();
        SeedMany(ctx, [
            MakePayment(EPaymentStatus.Pending),
            MakePayment(EPaymentStatus.Completed),
            MakePayment(EPaymentStatus.Pending)
        ]);
        var storage = new GetPaymentsStorage(ctx);

        var result = await storage.GetAsync(1, 10, EPaymentStatus.Pending, null, null, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result.Data!, p => Assert.Equal(EPaymentStatus.Pending, p.Status));
    }

    [Fact]
    public async Task GetAsync_FiltersByProvider()
    {
        using var ctx = CreateContext();
        SeedMany(ctx, [
            MakePayment(provider: EPaymentProvider.Stripe),
            MakePayment(provider: EPaymentProvider.PayPal),
            MakePayment(provider: EPaymentProvider.Stripe)
        ]);
        var storage = new GetPaymentsStorage(ctx);

        var result = await storage.GetAsync(1, 10, null, EPaymentProvider.Stripe, null, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result.Data!, p => Assert.Equal(EPaymentProvider.Stripe, p.PaymentProvider));
    }

    [Fact]
    public async Task GetAsync_FiltersByDateRange()
    {
        using var ctx = CreateContext();
        var old = DateTimeOffset.UtcNow.AddDays(-10);
        var recent = DateTimeOffset.UtcNow;
        SeedMany(ctx, [
            MakePayment(createdAt: old),
            MakePayment(createdAt: recent),
            MakePayment(createdAt: recent)
        ]);
        var storage = new GetPaymentsStorage(ctx);

        var from = DateTimeOffset.UtcNow.AddDays(-1);
        var result = await storage.GetAsync(1, 10, null, null, from, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAsync_PaginatesCorrectly()
    {
        using var ctx = CreateContext();
        SeedMany(ctx, Enumerable.Range(0, 5).Select(_ => MakePayment()));
        var storage = new GetPaymentsStorage(ctx);

        var result = await storage.GetAsync(1, 2, null, null, null, null, CancellationToken.None);

        Assert.Equal(5, result.Count);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetAsync_ReturnsEmpty_WhenNoMatch()
    {
        using var ctx = CreateContext();
        SeedMany(ctx, [MakePayment(EPaymentStatus.Pending)]);
        var storage = new GetPaymentsStorage(ctx);

        var result = await storage.GetAsync(1, 10, EPaymentStatus.Completed, null, null, null, CancellationToken.None);

        Assert.Equal(0, result.Count);
        Assert.Empty(result.Data!);
    }
}
