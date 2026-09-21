using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.RoomTypes
{
    public class RoomTypeSearchRequest
    {
        public Guid KhachSanId { get; set; }
        public int SoKhach { get; set; }
        public DateTime? NgayNhan { get; set; }
        public DateTime? NgayTra { get; set; }
    }
}
