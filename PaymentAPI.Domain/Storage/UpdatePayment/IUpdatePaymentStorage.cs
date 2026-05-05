using PaymentAPI.Domain.Enums;

namespace PaymentAPI.Domain.Storage.UpdatePayment
{
    public interface IUpdatePaymentStorage
    {
        Task UpdateStatusAsync(Guid id, EPaymentStatus status, Guid updatedBy, CancellationToken ct);
    }
}
