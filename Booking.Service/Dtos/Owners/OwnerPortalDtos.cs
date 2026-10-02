using System.ComponentModel.DataAnnotations;

namespace Booking.Service.Dtos.Owners
{
    public class OwnerDashboardResponse
    {
        public int TotalBookings { get; set; }
        public int TodayBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRoomsToday { get; set; }
        public decimal OccupancyRateToday { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueThisYear { get; set; }
    }

    public class OwnerRevenueReportResponse
    {
        public List<OwnerRevenuePoint> Data { get; set; } = new();
        public int TotalBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal CancellationRate { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class OwnerRevenuePoint
    {
        public string Period { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OwnerCalendarResponse
    {
        public List<OwnerCalendarBooking> Bookings { get; set; } = new();
        public List<RoomAvailabilityBlockResponse> Blocks { get; set; } = new();
    }

    public class OwnerCalendarBooking
    {
        public Guid BookingId { get; set; }
        public Guid? HotelId { get; set; }
        public string? HotelName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string? Status { get; set; }
        public List<string?> RoomNumbers { get; set; } = new();
    }

    public class CreateRoomAvailabilityBlockRequest
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        [StringLength(300)]
        public string? Reason { get; set; }
    }

    public class RoomAvailabilityBlockResponse
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string? Reason { get; set; }
    }
}