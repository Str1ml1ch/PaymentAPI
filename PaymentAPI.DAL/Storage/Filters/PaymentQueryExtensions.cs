using PaymentAPI.DAL.Entities;
using PaymentAPI.Core.Enums;

namespace PaymentAPI.DAL.Storage.Filters
{
    public static class PaymentQueryExtensions
    {
        public static IQueryable<Payment> FilterByStatus(this IQueryable<Payment> query, EPaymentStatus? status)
        {
            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);
            return query;
        }

        public static IQueryable<Payment> FilterByProvider(this IQueryable<Payment> query, EPaymentProvider? provider)
        {
            if (provider.HasValue)
                query = query.Where(p => p.PaymentProvider == provider.Value);
            return query;
        }

        public static IQueryable<Payment> FilterByDateRange(this IQueryable<Payment> query, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            if (fromDate.HasValue)
                query = query.Where(p => p.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(p => p.CreatedAt <= toDate.Value);
            return query;
        }
    }
}
