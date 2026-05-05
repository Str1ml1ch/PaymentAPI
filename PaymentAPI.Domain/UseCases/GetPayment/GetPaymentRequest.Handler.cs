using MediatR;
using PaymentAPI.Domain.Models;
using PaymentAPI.Domain.Exceptions;
using PaymentAPI.Domain.Storage.GetPaymentById;

namespace PaymentAPI.Domain.UseCases.GetPayment
{
    public class GetPaymentRequestHandler : IRequestHandler<GetPaymentRequest, PaymentModel>
    {
        private readonly IGetPaymentByIdStorage _storage;

        public GetPaymentRequestHandler(IGetPaymentByIdStorage storage)
        {
            _storage = storage;
        }

        public async Task<PaymentModel> Handle(GetPaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = await _storage.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment is null) throw new PaymentNotFoundException(request.PaymentId);
            return payment;
        }
    }
}
