using System.Linq.Expressions;

namespace PaymentAPI.DAL.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> ToExpression();
    }
}
