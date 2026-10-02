using Booking.Service.Dtos.Owners;

namespace Booking.Service.Services.Owners
{
    public interface IOwnerPortalService
    {
        Task<OwnerDashboardResponse?> GetDashboard(Guid ownerId, Guid? hotelId);
        Task<OwnerRevenueReportResponse?> GetRevenueReport(Guid ownerId, Guid? hotelId, DateTime from, DateTime to, string groupBy);
        Task<OwnerCalendarResponse?> GetCalendar(Guid ownerId, Guid hotelId, DateTime from, DateTime to);
        Task<RoomAvailabilityBlockResponse?> CreateRoomBlock(Guid ownerId, Guid roomId, CreateRoomAvailabilityBlockRequest request);
        Task<bool> DeleteRoomBlock(Guid ownerId, Guid blockId);
    }
}