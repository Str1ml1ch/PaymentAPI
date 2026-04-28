using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentAPI.Core.Storage.CreatePayment;
using PaymentAPI.DAL.Storage.CreatePayment;
using PaymentAPI.DAL.Storage.GetPaymentById;
using PaymentAPI.DAL.Storage.GetPayments;
using PaymentAPI.DAL.Storage.RemovePayment;
using PaymentAPI.DAL.Storage.UpdatePayment;

namespace PaymentAPI.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContextPool<PaymentDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddScoped<ICreatePaymentStorage, CreatePaymentStorage>()
                .AddScoped<IGetPaymentByIdStorage, GetPaymentByIdStorage>()
                .AddScoped<IGetPaymentsStorage, GetPaymentsStorage>()
                .AddScoped<IRemovePaymentStorage, RemovePaymentStorage>()
                .AddScoped<IUpdatePaymentStorage, UpdatePaymentStorage>();
        }
    }
}
