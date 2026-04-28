using MediatR;
using PaymentAPI.Core.Models;
using PaymentAPI.DAL.Storage.GetPaymentById;
using PaymentAPI.Domain.Exceptions;

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
