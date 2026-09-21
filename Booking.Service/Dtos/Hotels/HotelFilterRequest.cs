

namespace Booking.Service.Dtos.Hotels
{
    public class HotelFilterRequest
    {
        public string? Keyword { get; set; }
        public int? SoKhach { get; set; }
        public DateTime? NgayNhanPhong { get; set; }
        public DateTime? NgayTraPhong { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
