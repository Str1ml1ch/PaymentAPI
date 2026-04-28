namespace PaymentAPI.Domain.Services
{
    public class OrderApiClient : IOrderApiClient
    {
        private readonly IHttpClientFactory _factory;

        public OrderApiClient(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task CancelOrderAsync(Guid orderId, CancellationToken ct)
        {
            var client = _factory.CreateClient("OrderApi");
            var response = await client.PutAsync($"/api/orders/{orderId}/cancel", null, ct);
            response.EnsureSuccessStatusCode();
        }
    }
}
