namespace PaymentAPI.DAL.Storage.RemovePayment
{
    public interface IRemovePaymentStorage
    {
        Task RemoveByIdAsync(Guid id, CancellationToken ct);
    }
}
