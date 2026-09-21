using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.PhongDats;
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
        Task UpdateBookingStatus(DatPhong d);

        Task<(List<DatPhong> Items, int TotalCount)> GetOwnerBookings(
            Guid ownerId,
            Guid khachSanId,
            int pageIndex,
            int pageSize);
        Task<(List<DatPhong> Items, int TotalCount)> GetBookingsByOwner(Guid ownerId,
            int pageIndex,
            int pageSize);

        Task<(List<DatPhong> Items, int TotalCount)> GetBookingHistory(Guid userId, int pageIndex, int pageSize);
        Task<DatPhong> CheckIn(Guid id);
        Task<DatPhong> ConfirmBooking(Guid id);
        Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(Guid hotelId, DateTime batDau, DateTime ketThuc, string trangThai);
    }
}
