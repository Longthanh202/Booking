using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.DatPhongs
{
    public class DatPhong
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? KhachHangId { get; set; }
        public Guid? KhachSanId { get; set; }
        public DateTime? NgayNhanPhong { get; set; } // DATE -> DateTime trong C# (hoặc DateOnly)
        public DateTime? NgayTraPhong { get; set; }
        public decimal? TongTien { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public KhachSan? KhachSan { get; set; }

        public ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; }
            = new List<ChiTietDatPhong>();

        public ICollection<ThanhToan> ThanhToans { get; set; }
            = new List<ThanhToan>();
    }
}
