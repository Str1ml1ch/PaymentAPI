namespace PaymentAPI.Domain.Exceptions
{
    public class PaymentNotFoundException : NotFoundException
    {
        public PaymentNotFoundException(Guid id) : base($"Payment with id: {id} not found") { }
    }
}
