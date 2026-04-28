using Homework.Ticketing.System.Shared.Enums;
using MediatR;

namespace PaymentAPI.Domain.UseCases.CreatePayment
{
    public class CreatePaymentRequest : IRequest<Guid>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public ECurrency Currency { get; set; }
    }
}
