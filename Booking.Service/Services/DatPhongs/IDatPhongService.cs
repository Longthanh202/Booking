using Booking.Core.Model.DatPhongs;
using Booking.Service.Dtos.DatPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.DatPhongs
{
    public interface IDatPhongService
    {
        Task<DatPhong> CreateBooking(DatPhongRequest dp, Guid userId);
        Task<DatPhong> UpdateBookingStatus(Guid id);
        Task<DatPhongOwnerDto> GetOwnerBookings(DatPhongOwnerRequest dto, Guid ownerId);
        Task<DatPhongOwnerDto> GetBookingsByOwner(DatPhongOwner_v0 dto, Guid ownerId);

        Task<BookingHistory> GetBookingHistory(Guid userId, int pageIndex, int pageSize);
        Task CheckIn(Guid id);
        Task ConfirmBooking(Guid id);
        Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(ThongKeOwnerRequest input);
    }
}
