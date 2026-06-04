using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Entities;
using PaymentAPI.DAL.Storage.UpdatePayment;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Tests.DAL.Infrastructure;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.DAL;

[Collection("SqlServer")]
public class UpdatePaymentStorageTests : IAsyncLifetime
{
    private readonly SqlServerContainerFixture _fixture;
    private PaymentDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;

    public UpdatePaymentStorageTests(SqlServerContainerFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        _context = new PaymentDbContext(
            new DbContextOptionsBuilder<PaymentDbContext>()
                .UseSqlServer(_fixture.ConnectionString)
                .Options);
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _transaction.RollbackAsync();
        await _context.DisposeAsync();
    }

    private Payment Seed()
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
        _context.Payments.Add(p);
        _context.SaveChanges();
        return p;
    }

    [Fact]
    public async Task UpdateStatusAsync_ChangesStatus()
    {
        var payment = Seed();
        var storage = new UpdatePaymentStorage(_context);
        var updatedBy = Guid.NewGuid();

        await storage.UpdateStatusAsync(payment.Id, EPaymentStatus.Completed, updatedBy, CancellationToken.None);

        var updated = _context.Payments.Find(payment.Id)!;
        Assert.Equal(EPaymentStatus.Completed, updated.Status);
        Assert.Equal(updatedBy, updated.UpdatedBy);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_ToFailed_ChangesStatus()
    {
        var payment = Seed();
        var storage = new UpdatePaymentStorage(_context);

        await storage.UpdateStatusAsync(payment.Id, EPaymentStatus.Failed, Guid.NewGuid(), CancellationToken.None);

        var updated = _context.Payments.Find(payment.Id)!;
        Assert.Equal(EPaymentStatus.Failed, updated.Status);
    }
}
