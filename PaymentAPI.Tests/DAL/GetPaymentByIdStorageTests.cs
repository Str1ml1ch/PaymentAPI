using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Entities;
using PaymentAPI.DAL.Storage.GetPaymentById;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Tests.DAL.Infrastructure;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.DAL;

[Collection("SqlServer")]
public class GetPaymentByIdStorageTests : IAsyncLifetime
{
    private readonly SqlServerContainerFixture _fixture;
    private PaymentDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;

    public GetPaymentByIdStorageTests(SqlServerContainerFixture fixture) => _fixture = fixture;

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

    private Payment Seed(Guid? id = null, Guid? orderId = null, string txnId = "txn-1",
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
        _context.Payments.Add(p);
        _context.SaveChanges();
        return p;
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var storage = new GetPaymentByIdStorage(_context);

        var result = await storage.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsModel_WhenFound()
    {
        var payment = Seed();
        var storage = new GetPaymentByIdStorage(_context);

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
        var orderId = Guid.NewGuid();
        var payment = Seed(orderId: orderId);
        var storage = new GetPaymentByIdStorage(_context);

        var result = await storage.GetByOrderIdAsync(orderId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.OrderId);
    }

    [Fact]
    public async Task GetByExternalTransactionIdAsync_ReturnsModel_WhenFound()
    {
        Seed(txnId: "my-txn-999");
        var storage = new GetPaymentByIdStorage(_context);

        var result = await storage.GetByExternalTransactionIdAsync("my-txn-999", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("my-txn-999", result.ExternalPaymentTranscationId);
    }

    [Fact]
    public async Task IsExistsAsync_ReturnsFalse_WhenNotFound()
    {
        var storage = new GetPaymentByIdStorage(_context);

        var result = await storage.IsExistsAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task IsExistsAsync_ReturnsTrue_WhenFound()
    {
        var payment = Seed();
        var storage = new GetPaymentByIdStorage(_context);

        var result = await storage.IsExistsAsync(payment.Id, CancellationToken.None);

        Assert.True(result);
    }
}
