using Homework.Ticketing.System.Shared.Enums;
using PaymentAPI.Domain.Enums;

namespace PaymentAPI.Domain.Storage.CreatePayment
{
    public interface ICreatePaymentStorage
    {
        Task<Guid> CreateAsync(Guid orderId, string externalTransactionId, decimal amount, ECurrency currency, EPaymentStatus status, EPaymentProvider provider, CancellationToken ct);
    }
}
