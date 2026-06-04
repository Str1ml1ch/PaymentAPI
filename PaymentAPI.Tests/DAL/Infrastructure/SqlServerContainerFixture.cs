using Microsoft.EntityFrameworkCore;
using PaymentAPI.DAL;
using Testcontainers.MsSql;

namespace PaymentAPI.Tests.DAL.Infrastructure;

public sealed class SqlServerContainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        await using var context = new PaymentDbContext(options);
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();
}
