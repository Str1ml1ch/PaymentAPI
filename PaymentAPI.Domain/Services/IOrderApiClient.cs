namespace PaymentAPI.Domain.Services
{
    public interface IOrderApiClient
    {
        Task CancelOrderAsync(Guid orderId, CancellationToken ct);
    }
}
