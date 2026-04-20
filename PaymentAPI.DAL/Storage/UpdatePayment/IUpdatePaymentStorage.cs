using PaymentAPI.Core.Enums;

namespace PaymentAPI.DAL.Storage.UpdatePayment
{
    public interface IUpdatePaymentStorage
    {
        Task UpdateStatusAsync(Guid id, EPaymentStatus status, Guid updatedBy, CancellationToken ct);
    }
}
