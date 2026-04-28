using MediatR;

namespace PaymentAPI.Domain.UseCases.FailPayment
{
    public class FailPaymentRequest : IRequest<bool>
    {
        public Guid PaymentId { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
