using Homework.Ticketing.System.Shared.Enums;
using PaymentAPI.Core.Enums;
using Shared.DAL.Entities;

namespace PaymentAPI.DAL.Entities
{
    public class Payment : BaseDbEntity
    {
        public Guid OrderId { get; set; }
        public string ExternalPaymentTranscationId { get; set; } = null!;
        public decimal Amount { get; set; }
        public ECurrency Currency { get; set; }
        public EPaymentStatus Status { get; set; }
        public EPaymentProvider PaymentProvider { get; set; }

    }
}
