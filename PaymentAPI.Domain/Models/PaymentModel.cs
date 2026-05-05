using Homework.Ticketing.System.Shared.Enums;
using PaymentAPI.Domain.Enums;

namespace PaymentAPI.Domain.Models
{
    public class PaymentModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ExternalPaymentTranscationId { get; set; } = null!;
        public decimal Amount { get; set; }
        public ECurrency Currency { get; set; }
        public EPaymentStatus Status { get; set; }
        public EPaymentProvider PaymentProvider { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
