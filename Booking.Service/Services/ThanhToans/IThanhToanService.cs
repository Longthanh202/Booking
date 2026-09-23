using Booking.Core.Model.DatPhongs;
using Booking.Service.Dtos.Payments;

namespace Booking.Service.Services.ThanhToans
{
    public interface IThanhToanService
    {
        Task<ThanhToan?> GetForCustomer(Guid paymentId, Guid customerId);
        Task<List<ThanhToan>> GetAll();
        Task<ThanhToan?> UpdateStatus(Guid paymentId, UpdatePaymentStatusRequest request);
    }
}