using Container_App.Core.Model.ChiTietChiTraKhachSans;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.TaiKhoanNganHangs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.ChiTraKhachSans
{
    public class ChiTraKhachSan
    {
        public long MaChiTra { get; set; }

        public Guid KhachSanId { get; set; }

        public Guid TaiKhoanNganHangId { get; set; }

        public decimal SoTien { get; set; }

        public string TrangThai { get; set; }

        public string? MaGiaoDich { get; set; }

        public string? GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayChiTra { get; set; }

        #region Navigation

        public virtual KhachSan KhachSan { get; set; }

        public virtual TaiKhoanNganHang TaiKhoanNganHang { get; set; }

        public virtual ICollection<ChiTietChiTraKhachSan> ChiTietChiTraKhachSans { get; set; } = new List<ChiTietChiTraKhachSan>();

        #endregion
    }
}
