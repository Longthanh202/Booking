using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.QuangCaos
{
    public class KhachSanQuangCao
    {
        public long Id { get; set; }
        public Guid? KhachSanId { get; set; }
        public int? GoiQuanCaoId { get; set; } // Lưu ý chữ QuanCao/QuangCao theo DB của bạn
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? DiemUuTien { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Navigations
        public virtual KhachSan? KhachSan { get; set; }
        public virtual GoiQuangCao? GoiQuangCao { get; set; }
    }
}
