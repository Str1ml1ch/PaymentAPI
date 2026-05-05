using Homework.Ticketing.System.Shared.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Models;
using PaymentAPI.DAL.Specifications.Payments;
using PaymentAPI.Domain.Storage.GetPayments;

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
            var query = _context.Payments.AsQueryable();
            if (status.HasValue)
                query = query.Where(new PaymentByStatusSpecification(status.Value).ToExpression());
            if (provider.HasValue)
                query = query.Where(new PaymentByProviderSpecification(provider.Value).ToExpression());
            if (fromDate.HasValue || toDate.HasValue)
                query = query.Where(new PaymentByDateRangeSpecification(fromDate, toDate).ToExpression());

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
