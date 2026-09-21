using Booking.Core.Model.KhachSanImage;
using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.TienIchs;
using Booking.Service.Dtos.HotelImages;
using Booking.Service.Dtos.RoomTypes;
using Booking.Service.Dtos.Amenities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Hotels
{
    public class HotelDetailsResponse
    {
        public Guid Id { get; set; }
        public string? TenKhachSan { get; set; } = string.Empty;
        public string? MoTa { get; set; } = string.Empty;
        public string? DiaChi { get; set; } = string.Empty;
        public int? SoSao { get; set; }
        public TimeSpan? GioNhanPhong { get; set; }
        public TimeSpan? GioTraPhong { get; set; }
        public string? TenThanhPho { get; set; } = string.Empty; // Đổi tên rõ nghĩa hơn full_name     
        public List<AmenityDto> TienIchs { get; set; } = new();
        public List<HotelImageDto> KhachSanImages { get; set; } = new();
    }
}
