using Homework.Ticketing.System.Shared.Enums;
using PaymentAPI.Domain.Enums;
using PaymentAPI.DAL.Entities;
using PaymentAPI.Domain.Storage.CreatePayment;

namespace PaymentAPI.DAL.Storage.CreatePayment
{
    public class CreatePaymentStorage : ICreatePaymentStorage
    {
        private readonly PaymentDbContext _context;

        public CreatePaymentStorage(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(Guid orderId, string externalTransactionId, decimal amount, ECurrency currency, EPaymentStatus status, EPaymentProvider provider, CancellationToken ct)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ExternalPaymentTranscationId = externalTransactionId,
                Amount = amount,
                Currency = currency,
                Status = status,
                PaymentProvider = provider,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(ct);

            return payment.Id;
        }
    }
}
