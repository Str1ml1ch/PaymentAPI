using MediatR;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Storage.CreatePayment;

namespace PaymentAPI.Domain.UseCases.CreatePayment
{
    public class CreatePaymentRequestHandler : IRequestHandler<CreatePaymentRequest, Guid>
    {
        private readonly ICreatePaymentStorage _storage;

        public CreatePaymentRequestHandler(ICreatePaymentStorage storage)
        {
            _storage = storage;
        }

        public async Task<Guid> Handle(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            return await _storage.CreateAsync(
                request.OrderId,
                Guid.NewGuid().ToString(),
                request.Amount,
                request.Currency,
                EPaymentStatus.Pending,
                EPaymentProvider.Stripe,
                cancellationToken);
        }
    }
}
