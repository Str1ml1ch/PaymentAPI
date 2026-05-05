using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Enums;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Entities;
using PaymentAPI.DAL.Storage.UpdatePayment;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.DAL;

public class UpdatePaymentStorageTests
{
    private static PaymentDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PaymentDbContext(options);
    }

    private static Payment Seed(PaymentDbContext ctx)
    {
        var p = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            ExternalPaymentTranscationId = "txn-x",
            Amount = 120m,
            Currency = ECurrency.USD,
            Status = EPaymentStatus.Pending,
            PaymentProvider = EPaymentProvider.Stripe,
            CreatedAt = DateTimeOffset.UtcNow
        };
        ctx.Payments.Add(p);
        ctx.SaveChanges();
        return p;
    }

    [Fact]
    public async Task UpdateStatusAsync_ChangesStatus()
    {
        using var ctx = CreateContext();
        var payment = Seed(ctx);
        var storage = new UpdatePaymentStorage(ctx);
        var updatedBy = Guid.NewGuid();

        await storage.UpdateStatusAsync(payment.Id, EPaymentStatus.Completed, updatedBy, CancellationToken.None);

        var updated = ctx.Payments.Find(payment.Id)!;
        Assert.Equal(EPaymentStatus.Completed, updated.Status);
        Assert.Equal(updatedBy, updated.UpdatedBy);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_ToFailed_ChangesStatus()
    {
        using var ctx = CreateContext();
        var payment = Seed(ctx);
        var storage = new UpdatePaymentStorage(ctx);

        await storage.UpdateStatusAsync(payment.Id, EPaymentStatus.Failed, Guid.NewGuid(), CancellationToken.None);

        var updated = ctx.Payments.Find(payment.Id)!;
        Assert.Equal(EPaymentStatus.Failed, updated.Status);
    }
}
