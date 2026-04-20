using Homework.Ticketing.System.Shared.Models;
using PaymentAPI.Core.Enums;
using PaymentAPI.Core.Models;

namespace PaymentAPI.DAL.Storage.GetPayments
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
