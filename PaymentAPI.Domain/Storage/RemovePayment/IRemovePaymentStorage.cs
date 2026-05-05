namespace PaymentAPI.Domain.Storage.RemovePayment
{
    public interface IRemovePaymentStorage
    {
        Task RemoveByIdAsync(Guid id, CancellationToken ct);
    }
}
