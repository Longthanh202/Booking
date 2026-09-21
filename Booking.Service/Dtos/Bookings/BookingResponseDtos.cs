using Booking.Service.Dtos.Common;

namespace Booking.Service.Dtos.Bookings
{

    public class OwnerBookingResponse : PagedResponse<BookingDto>
    {
    }

    public class BookingHistoryResponse : PagedResponse<BookingDto>
    {
    }

    public class BookingDto
    {
        public Guid Id { get; set; }

        public string? TrangThai { get; set; }

        public string? ThanhToan { get; set; }

        public Guid? KhachSanId { get; set; }

        public string? TenKhachSan { get; set; }

        public DateTime? NgayTao { get; set; }
        public DateTime? NgayNhan { get; set; }
        public DateTime? NgayTra { get; set; }

        public List<BookingRoomTypeDetailDto> ChiTietDatPhongs { get; set; } = new();
    }

    public class BookingRoomTypeDetailDto
    {
        public Guid Id { get; set; }

        public Guid? LoaiPhongId { get; set; }

        public string? TenLoaiPhong { get; set; }

        public decimal? Gia { get; set; }

        public List<BookedRoomDto> Phongs { get; set; } = new();
    }

    public class BookedRoomDto
    {
        public Guid Id { get; set; }

        public string? SoPhong { get; set; }

        public string? TrangThai { get; set; }
    }

    public class OwnerBookingSearchRequest
    {
        public Guid KhachSanId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class OwnerBookingListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}