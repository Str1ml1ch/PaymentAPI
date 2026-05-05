using PaymentAPI.Domain.Models;

namespace PaymentAPI.Domain.Storage.GetPaymentById
{
    public interface IGetPaymentByIdStorage
    {
        Task<PaymentModel?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<PaymentModel?> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
        Task<PaymentModel?> GetByExternalTransactionIdAsync(string externalTransactionId, CancellationToken ct);
        Task<bool> IsExistsAsync(Guid id, CancellationToken ct);
    }
}
