using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Storage.RemovePayment;

namespace PaymentAPI.DAL.Storage.RemovePayment
{
    public class RemovePaymentStorage : IRemovePaymentStorage
    {
        private readonly PaymentDbContext _context;

        public RemovePaymentStorage(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task RemoveByIdAsync(Guid id, CancellationToken ct)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);

            _context.Payments.Remove(payment!);
            await _context.SaveChangesAsync(ct);
        }
    }
}
