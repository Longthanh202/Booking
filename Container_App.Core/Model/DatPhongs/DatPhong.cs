using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.DatPhongs
{
    public class DatPhong
    {
        public Guid Id { get; set; }

        public Guid KhachHangId { get; set; }

        public Guid KhachSanId { get; set; }

        public DateTime NgayNhanPhong { get; set; }

        public DateTime NgayTraPhong { get; set; }

        public decimal TongTien { get; set; }    

        /// <summary>
        /// CHO_THANH_TOAN
        /// DA_XAC_NHAN
        /// DA_HUY
        /// HOAN_THANH
        /// </summary>
        public string TrangThai { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; }

        public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; }
            = new List<ChiTietDatPhong>();

        public ICollection<ThanhToan> ThanhToans { get; set; }
            = new List<ThanhToan>();
    }
}
