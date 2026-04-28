using System.Linq.Expressions;
using PaymentAPI.Core.Enums;
using PaymentAPI.DAL.Entities;

namespace PaymentAPI.DAL.Specifications.Payments
{
    public sealed class PaymentByStatusSpecification : ISpecification<Payment>
    {
        private readonly EPaymentStatus _status;
        public PaymentByStatusSpecification(EPaymentStatus status) => _status = status;
        public Expression<Func<Payment, bool>> ToExpression() => p => p.Status == _status;
    }
}
