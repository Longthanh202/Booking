using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.DatPhongs
{
    public class DatPhongEvent
    {
        public Guid BookingId { get; set; }

        public string HoTen { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string TenKhachSan { get; set; } = string.Empty;

        public DateTime NgayNhanPhong { get; set; }

        public DateTime NgayTraPhong { get; set; }

        public decimal TongTien { get; set; }
    }
}
