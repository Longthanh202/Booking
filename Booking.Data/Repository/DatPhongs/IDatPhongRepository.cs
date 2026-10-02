using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.PhongDats;
using Booking.Core.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.DatPhongs
{
    public interface IDatPhongRepository
    {
        Task<DatPhong> CreateBooking(DatPhong dp, List<ChiTietDatPhong> ctdp, List<PhongDat> phongDats);
        Task<DatPhong?> GetBookingById(Guid id);
        Task<DatPhong?> GetCustomerBookingById(Guid id, Guid customerId);
        Task<bool> CancelBooking(Guid id, Guid customerId);
        Task<bool> ChangeOwnerBookingStatus(Guid id, Guid ownerId, string status);
        Task<bool> IsBookingOwnedBy(Guid bookingId, Guid ownerId);
        Task<bool> IsRoomTypeForHotel(Guid roomTypeId, Guid hotelId);
        Task UpdateBookingStatus(DatPhong d);

        Task<(List<DatPhong> Items, int TotalCount)> GetOwnerBookings(
            Guid ownerId,
            Guid khachSanId,
            int pageIndex,
            int pageSize);
        Task<(List<DatPhong> Items, int TotalCount)> GetBookingsByOwner(
            Guid ownerId, 
            Guid? hotelId, 
            Guid? customerId,
            string? trangThai, 
            DateTime? ngayTao,
            int pageIndex,
            int pageSize);

        Task<(List<DatPhong> Items, int TotalCount)> GetBookingHistory(Guid userId, int pageIndex, int pageSize);
        Task<List<UserProfile>> GetCustomerOptionsByHotelId(Guid hotelId, Guid ownerId);
        Task<DatPhong> CheckIn(Guid id);
        Task<DatPhong> ConfirmBooking(Guid id);
        Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(Guid hotelId, DateTime batDau, DateTime ketThuc, string trangThai);
    }
}
