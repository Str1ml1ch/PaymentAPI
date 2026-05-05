using MediatR;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Exceptions;
using PaymentAPI.Domain.Storage.GetPaymentById;
using PaymentAPI.Domain.Storage.UpdatePayment;

namespace PaymentAPI.Domain.UseCases.CompletePayment
{
    public class CompletePaymentRequestHandler : IRequestHandler<CompletePaymentRequest, bool>
    {
        private readonly IGetPaymentByIdStorage _getStorage;
        private readonly IUpdatePaymentStorage _updateStorage;

        public CompletePaymentRequestHandler(IGetPaymentByIdStorage getStorage, IUpdatePaymentStorage updateStorage)
        {
            _getStorage = getStorage;
            _updateStorage = updateStorage;
        }

        public async Task<bool> Handle(CompletePaymentRequest request, CancellationToken cancellationToken)
        {
            var exists = await _getStorage.IsExistsAsync(request.PaymentId, cancellationToken);
            if (!exists) throw new PaymentNotFoundException(request.PaymentId);

            await _updateStorage.UpdateStatusAsync(request.PaymentId, EPaymentStatus.Completed, request.UpdatedBy, cancellationToken);
            return true;
        }
    }
}
