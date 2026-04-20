using Homework.Ticketing.System.Shared.Enums;
using PaymentAPI.Core.Enums;

namespace PaymentAPI.Core.Storage.CreatePayment
{
    public interface ICreatePaymentStorage
    {
        Task<Guid> CreateAsync(Guid orderId, string externalTransactionId, decimal amount, ECurrency currency, EPaymentStatus status, EPaymentProvider provider, CancellationToken ct);
    }
}
