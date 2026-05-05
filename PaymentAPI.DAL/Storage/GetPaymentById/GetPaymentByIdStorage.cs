using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Models;
using PaymentAPI.Domain.Storage.GetPaymentById;

namespace PaymentAPI.DAL.Storage.GetPaymentById
{
    public class GetPaymentByIdStorage : IGetPaymentByIdStorage
    {
        private readonly PaymentDbContext _context;

        public GetPaymentByIdStorage(PaymentDbContext context)
        {
            _context = context;
        }

        private static System.Linq.Expressions.Expression<Func<Entities.Payment, PaymentModel>> ToModel =>
            p => new PaymentModel
            {
                Id = p.Id,
                OrderId = p.OrderId,
                ExternalPaymentTranscationId = p.ExternalPaymentTranscationId,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status,
                PaymentProvider = p.PaymentProvider,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };

        public async Task<PaymentModel?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Payments
                .Where(p => p.Id == id)
                .Select(ToModel)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PaymentModel?> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .Select(ToModel)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PaymentModel?> GetByExternalTransactionIdAsync(string externalTransactionId, CancellationToken ct)
        {
            return await _context.Payments
                .Where(p => p.ExternalPaymentTranscationId == externalTransactionId)
                .Select(ToModel)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsExistsAsync(Guid id, CancellationToken ct)
        {
            return await _context.Payments.AnyAsync(p => p.Id == id, ct);
        }
    }
}
