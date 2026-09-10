

namespace Booking.Service.Dtos.KhachSanDto
{
    public class FilterHotelRequestDto
    {
        public string? Keyword { get; set; }
        public int? SoKhach { get; set; }
        public DateTime? NgayNhanPhong { get; set; }
        public DateTime? NgayTraPhong { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
