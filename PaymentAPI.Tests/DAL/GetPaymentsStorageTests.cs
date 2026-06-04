using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Entities;
using PaymentAPI.DAL.Storage.GetPayments;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Tests.DAL.Infrastructure;
using Homework.Ticketing.System.Shared.Enums;

namespace PaymentAPI.Tests.DAL;

[Collection("SqlServer")]
public class GetPaymentsStorageTests : IAsyncLifetime
{
    private readonly SqlServerContainerFixture _fixture;
    private PaymentDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;

    public GetPaymentsStorageTests(SqlServerContainerFixture fixture) => _fixture = fixture;

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

    private void SeedMany(IEnumerable<Payment> payments)
    {
        _context.Payments.AddRange(payments);
        _context.SaveChanges();
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
        SeedMany([MakePayment(), MakePayment(), MakePayment()]);
        var storage = new GetPaymentsStorage(_context);

        var result = await storage.GetAsync(1, 10, null, null, null, null, CancellationToken.None);

        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Data!.Count);
    }

    [Fact]
    public async Task GetAsync_FiltersByStatus()
    {
        SeedMany([
            MakePayment(EPaymentStatus.Pending),
            MakePayment(EPaymentStatus.Completed),
            MakePayment(EPaymentStatus.Pending)
        ]);
        var storage = new GetPaymentsStorage(_context);

        var result = await storage.GetAsync(1, 10, EPaymentStatus.Pending, null, null, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result.Data!, p => Assert.Equal(EPaymentStatus.Pending, p.Status));
    }

    [Fact]
    public async Task GetAsync_FiltersByProvider()
    {
        SeedMany([
            MakePayment(provider: EPaymentProvider.Stripe),
            MakePayment(provider: EPaymentProvider.PayPal),
            MakePayment(provider: EPaymentProvider.Stripe)
        ]);
        var storage = new GetPaymentsStorage(_context);

        var result = await storage.GetAsync(1, 10, null, EPaymentProvider.Stripe, null, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result.Data!, p => Assert.Equal(EPaymentProvider.Stripe, p.PaymentProvider));
    }

    [Fact]
    public async Task GetAsync_FiltersByDateRange()
    {
        var old = DateTimeOffset.UtcNow.AddDays(-10);
        var recent = DateTimeOffset.UtcNow;
        SeedMany([
            MakePayment(createdAt: old),
            MakePayment(createdAt: recent),
            MakePayment(createdAt: recent)
        ]);
        var storage = new GetPaymentsStorage(_context);

        var from = DateTimeOffset.UtcNow.AddDays(-1);
        var result = await storage.GetAsync(1, 10, null, null, from, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAsync_PaginatesCorrectly()
    {
        SeedMany(Enumerable.Range(0, 5).Select(_ => MakePayment()));
        var storage = new GetPaymentsStorage(_context);

        var result = await storage.GetAsync(1, 2, null, null, null, null, CancellationToken.None);

        Assert.Equal(5, result.Count);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetAsync_ReturnsEmpty_WhenNoMatch()
    {
        SeedMany([MakePayment(EPaymentStatus.Pending)]);
        var storage = new GetPaymentsStorage(_context);

        var result = await storage.GetAsync(1, 10, EPaymentStatus.Completed, null, null, null, CancellationToken.None);

        Assert.Equal(0, result.Count);
        Assert.Empty(result.Data!);
    }
}
