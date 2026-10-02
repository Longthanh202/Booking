using Booking.Core.Model.DatPhongs;
using Booking.Service.Dtos.Bookings;
using Booking.Service.Dtos.Common;
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
        Task<DatPhong> UpdateBookingStatus(Guid id, Guid ownerId);
        Task<OwnerBookingResponse> GetOwnerBookings(OwnerBookingSearchRequest dto, Guid ownerId);
        Task<OwnerBookingResponse> GetBookingsByOwner(OwnerBookingListRequest dto, Guid ownerId);
        Task<OwnerBookingResponse> GetAdminBookings(OwnerBookingListRequest dto);

        Task<BookingHistoryResponse> GetBookingHistory(Guid userId, int pageIndex, int pageSize);
        Task<List<OptionDto>> GetCustomerOptionsByHotelId(Guid hotelId, Guid ownerId);
        Task<DatPhong?> GetCustomerBookingById(Guid id, Guid customerId);
        Task<bool> CancelBooking(Guid id, Guid customerId);
        Task<bool> RejectBooking(Guid id, Guid ownerId);
        Task<bool> CancelBookingByOwner(Guid id, Guid ownerId);
        Task CheckIn(Guid id, Guid ownerId);
        Task ConfirmBooking(Guid id, Guid ownerId);
        Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(OwnerBookingStatisticsRequest input);
    }
}
