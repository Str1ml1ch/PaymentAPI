using System.Linq.Expressions;
using PaymentAPI.Core.Enums;
using PaymentAPI.DAL.Entities;

namespace PaymentAPI.DAL.Specifications.Payments
{
    public sealed class PaymentByProviderSpecification : ISpecification<Payment>
    {
        private readonly EPaymentProvider _provider;
        public PaymentByProviderSpecification(EPaymentProvider provider) => _provider = provider;
        public Expression<Func<Payment, bool>> ToExpression() => p => p.PaymentProvider == _provider;
    }
}
