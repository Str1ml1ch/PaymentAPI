using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PaymentAPI.DAL;
using PaymentAPI.DAL.Storage.CreatePayment;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Storage.CreatePayment;
using PaymentAPI.Tests.DAL.Infrastructure;

namespace PaymentAPI.Tests.DAL;

[Collection("SqlServer")]
public class CreatePaymentStorageTests : IAsyncLifetime
{
    private readonly SqlServerContainerFixture _fixture;
    private PaymentDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;

    public CreatePaymentStorageTests(SqlServerContainerFixture fixture) => _fixture = fixture;

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

    [Fact]
    public async Task CreateAsync_PersistsPayment_AndReturnsNewId()
    {
        var storage = new CreatePaymentStorage(_context);
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
        var entity = _context.Payments.Single(p => p.Id == id);
        Assert.Equal(orderId, entity.OrderId);
        Assert.Equal("txn-001", entity.ExternalPaymentTranscationId);
        Assert.Equal(99.99m, entity.Amount);
        Assert.Equal(EPaymentStatus.Pending, entity.Status);
        Assert.Equal(EPaymentProvider.Stripe, entity.PaymentProvider);
    }
}
