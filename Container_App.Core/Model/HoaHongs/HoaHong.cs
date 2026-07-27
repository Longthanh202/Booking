using Container_App.Core.Model.ChiTietHoaDonHoaHongs;
using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.HoaHongs
{
    public class HoaHong
    {
        public long MaHoaHong { get; set; }

        public Guid MaDatPhong { get; set; }

        public Guid MaKhachSan { get; set; }

        public decimal TyLeHoaHong { get; set; }

        public decimal SoTienHoaHong { get; set; }

        public string? TrangThai { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayThu { get; set; }

        #region Navigation

        public virtual DatPhong DatPhong { get; set; }

        public virtual KhachSan KhachSan { get; set; }

        public virtual ICollection<ChiTietHoaDonHoaHong> ChiTietHoaDonHoaHongs { get; set; } = new List<ChiTietHoaDonHoaHong>();

        #endregion
    }
}
