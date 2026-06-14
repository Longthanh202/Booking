using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.DatPhongs
{
    public class ThanhToan
    {
        public Guid Id { get; set; }

        public Guid DatPhongId { get; set; }

        /// <summary>
        /// VNPAY
        /// MOMO
        /// THE
        /// </summary>
        public string PhuongThuc { get; set; } = string.Empty;

        public decimal SoTien { get; set; }

        /// <summary>
        /// CHO_THANH_TOAN
        /// THANH_CONG
        /// THAT_BAI
        /// HOAN_TIEN
        /// </summary>
        public string TrangThai { get; set; } = string.Empty;

        public DateTime? ThoiGianThanhToan { get; set; }

        public DatPhong DatPhong { get; set; } = null!;
    }
}
