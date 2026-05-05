using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Storage.UpdatePayment;

namespace PaymentAPI.DAL.Storage.UpdatePayment
{
    public class UpdatePaymentStorage : IUpdatePaymentStorage
    {
        private readonly PaymentDbContext _context;

        public UpdatePaymentStorage(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task UpdateStatusAsync(Guid id, EPaymentStatus status, Guid updatedBy, CancellationToken ct)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);

            payment!.Status = status;
            payment.UpdatedAt = DateTimeOffset.UtcNow;
            payment.UpdatedBy = updatedBy;

            await _context.SaveChangesAsync(ct);
        }
    }
}
