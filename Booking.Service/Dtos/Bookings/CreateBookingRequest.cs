using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.Bookings
{
    public class CreateBookingRequest
    {
        public Guid KhachSanId { get; set; }
        public DateTime? NgayNhanPhong { get; set; }
        public DateTime? NgayTraPhong { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public List<RoomQuantityRequest>? DanhSachPhong { get; set; }
    }

    public class RoomQuantityRequest
    {
        public Guid LoaiPhongId { get; set; }
        public int SoLuong { get; set; }
    }
}
