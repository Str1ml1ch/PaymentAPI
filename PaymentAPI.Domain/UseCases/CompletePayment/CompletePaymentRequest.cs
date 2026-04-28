using MediatR;

namespace PaymentAPI.Domain.UseCases.CompletePayment
{
    public class CompletePaymentRequest : IRequest<bool>
    {
        public Guid PaymentId { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
