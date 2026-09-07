using Booking.Core.Model.ChiTietHoaDonHoaHongs;
using Booking.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.HoaDonHoaHongs
{
    public class HoaDonHoaHong
    {
        public long Id { get; set; }

        public Guid KhachSanId { get; set; }

        public DateTime TuNgay { get; set; }

        public DateTime DenNgay { get; set; }

        public decimal TongTienHoaHong { get; set; }

        public string TrangThai { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        #region Navigation

        public virtual KhachSan KhachSan { get; set; }

        public virtual ICollection<ChiTietHoaDonHoaHong> ChiTietHoaDonHoaHongs { get; set; } = new List<ChiTietHoaDonHoaHong>();

        #endregion
    }
}
