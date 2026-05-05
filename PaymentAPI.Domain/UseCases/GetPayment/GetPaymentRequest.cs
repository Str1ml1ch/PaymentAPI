using MediatR;
using PaymentAPI.Domain.Models;

namespace PaymentAPI.Domain.UseCases.GetPayment
{
    public class GetPaymentRequest : IRequest<PaymentModel>
    {
        public Guid PaymentId { get; set; }
    }
}
