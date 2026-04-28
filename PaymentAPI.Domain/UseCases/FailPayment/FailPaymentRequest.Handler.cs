using MediatR;
using PaymentAPI.Core.Enums;
using PaymentAPI.DAL.Storage.GetPaymentById;
using PaymentAPI.DAL.Storage.UpdatePayment;
using PaymentAPI.Domain.Exceptions;
using PaymentAPI.Domain.Services;

namespace PaymentAPI.Domain.UseCases.FailPayment
{
    public class FailPaymentRequestHandler : IRequestHandler<FailPaymentRequest, bool>
    {
        private readonly IGetPaymentByIdStorage _getStorage;
        private readonly IUpdatePaymentStorage _updateStorage;
        private readonly IOrderApiClient _orderClient;

        public FailPaymentRequestHandler(
            IGetPaymentByIdStorage getStorage,
            IUpdatePaymentStorage updateStorage,
            IOrderApiClient orderClient)
        {
            _getStorage = getStorage;
            _updateStorage = updateStorage;
            _orderClient = orderClient;
        }

        public async Task<bool> Handle(FailPaymentRequest request, CancellationToken cancellationToken)
        {
            var payment = await _getStorage.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment is null) throw new PaymentNotFoundException(request.PaymentId);

            await _updateStorage.UpdateStatusAsync(request.PaymentId, EPaymentStatus.Failed, request.UpdatedBy, cancellationToken);
            await _orderClient.CancelOrderAsync(payment.OrderId, cancellationToken);
            return true;
        }
    }
}
