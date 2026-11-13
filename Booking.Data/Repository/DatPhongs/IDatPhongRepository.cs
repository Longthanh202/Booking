using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.PhongDats;
using Booking.Core.Model.Users;

namespace Booking.Data.Repository.DatPhongs
{
    public interface IDatPhongRepository
    {
        Task<DatPhong> CreateBooking(DatPhong dp, List<ChiTietDatPhong> ctdp, List<PhongDat> phongDats);
        Task<(int DeletedCount, int SkippedReferencedCount)> DeleteExpiredQrBookings(
            DateTime createdBefore,
            CancellationToken cancellationToken);
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
            string? maDatPhong,
            int pageIndex,
            int pageSize);
        Task<(List<DatPhong> Items, int TotalCount)> GetBookingsByOwner(
            Guid ownerId, 
            Guid? hotelId, 
            Guid? customerId,
            string? trangThai, 
            DateTime? ngayBatDau,
            DateTime? ngayKetThuc,
            string? maDatPhong,
            int pageIndex,
            int pageSize);

        Task<(List<DatPhong> Items, int TotalCount)> GetBookingHistory(
            Guid userId,
            string? maDatPhong,
            int pageIndex,
            int pageSize);
        Task<List<NguoiDung>> GetCustomerOptionsByHotelId(Guid hotelId, Guid ownerId);
        Task<DatPhong> CheckIn(Guid id);
        Task<DatPhong> ConfirmBooking(Guid id);
        Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(Guid hotelId, DateTime batDau, DateTime ketThuc, string trangThai);
    }
}
