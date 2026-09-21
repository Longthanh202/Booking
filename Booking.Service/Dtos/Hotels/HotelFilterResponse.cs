using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Hotels
{
    public class HotelFilterResponse
    {
        public List<HotelFilterItem> Data { get; set; } = new();
        public int TotalRow { get; set; }
        public int TotalPage { get; set; }
    }
    public class HotelFilterItem
    {
        public Guid Id { get; set; }
        public string TenKhachSan { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? DiaChi { get; set; }
        public int? SoSao { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public decimal? Gia { get; set; }
        public List<string> Urls { get; set; } = new();
    }
}
