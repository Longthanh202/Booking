using Booking.Core.Model.DatPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.ThanhToans
{
    public interface IThanhToanRepository
    {
        Task<ThanhToan> CreatePayment(ThanhToan t);
        Task<ThanhToan?> LayTheoDatPhong(Guid datPhongId);
        Task<ThanhToan?> GetForCustomer(Guid paymentId, Guid customerId);
        Task<ThanhToan?> GetForCustomerBooking(Guid bookingId, Guid customerId);
        Task<List<ThanhToan>> GetAll();
        Task<ThanhToan?> UpdateStatus(Guid paymentId, string status);
    }
}
