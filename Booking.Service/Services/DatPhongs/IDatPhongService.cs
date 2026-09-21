using Booking.Core.Model.DatPhongs;
using Booking.Service.Dtos.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.DatPhongs
{
    public interface IDatPhongService
    {
        Task<DatPhong> CreateBooking(CreateBookingRequest dp, Guid userId);
        Task<DatPhong> UpdateBookingStatus(Guid id);
        Task<OwnerBookingResponse> GetOwnerBookings(OwnerBookingSearchRequest dto, Guid ownerId);
        Task<OwnerBookingResponse> GetBookingsByOwner(OwnerBookingListRequest dto, Guid ownerId);

        Task<BookingHistoryResponse> GetBookingHistory(Guid userId, int pageIndex, int pageSize);
        Task CheckIn(Guid id);
        Task ConfirmBooking(Guid id);
        Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(OwnerBookingStatisticsRequest input);
    }
}
