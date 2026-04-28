using System.Linq.Expressions;
using PaymentAPI.DAL.Entities;

namespace PaymentAPI.DAL.Specifications.Payments
{
    public sealed class PaymentByDateRangeSpecification : ISpecification<Payment>
    {
        private readonly DateTimeOffset? _fromDate;
        private readonly DateTimeOffset? _toDate;

        public PaymentByDateRangeSpecification(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            _fromDate = fromDate;
            _toDate = toDate;
        }

        public Expression<Func<Payment, bool>> ToExpression()
            => p => (!_fromDate.HasValue || p.CreatedAt >= _fromDate.Value)
                 && (!_toDate.HasValue || p.CreatedAt <= _toDate.Value);
    }
}
