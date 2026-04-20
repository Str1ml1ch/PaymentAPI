using Homework.Ticketing.System.Shared.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Core.Enums;
using PaymentAPI.Core.Models;
using PaymentAPI.DAL.Storage.Filters;

namespace PaymentAPI.DAL.Storage.GetPayments
{
    public class GetPaymentsStorage : IGetPaymentsStorage
    {
        private readonly PaymentDbContext _context;

        public GetPaymentsStorage(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<List<PaymentModel>>> GetAsync(
            int page,
            int pageSize,
            EPaymentStatus? status,
            EPaymentProvider? provider,
            DateTimeOffset? fromDate,
            DateTimeOffset? toDate,
            CancellationToken ct)
        {
            var query = _context.Payments
                .FilterByStatus(status)
                .FilterByProvider(provider)
                .FilterByDateRange(fromDate, toDate);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PaymentModel
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
                })
                .ToListAsync(ct);

            return new ResultModel<List<PaymentModel>> { Data = items, Count = totalCount };
        }
    }
}
