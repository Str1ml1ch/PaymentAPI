using Homework.Ticketing.System.Shared.Models;
using PaymentAPI.Domain.Enums;
using PaymentAPI.Domain.Models;

namespace PaymentAPI.Domain.Storage.GetPayments
{
    public interface IGetPaymentsStorage
    {
        Task<ResultModel<List<PaymentModel>>> GetAsync(
            int page,
            int pageSize,
            EPaymentStatus? status,
            EPaymentProvider? provider,
            DateTimeOffset? fromDate,
            DateTimeOffset? toDate,
            CancellationToken ct);
    }
}
