using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.GiaPhongs;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.Phongs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.LoaiPhongs
{
    public class LoaiPhong
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? KhachSanId { get; set; }
        public string? TenLoaiPhong { get; set; }
        public int? SoKhachToiDa { get; set; }
        public string? KieuGiuong { get; set; }
        public string? MoTa { get; set; }
        public DateTime? NgayTao { get; set; }
        public virtual KhachSan? KhachSan { get; set; }
        public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
        public virtual ICollection<GiaPhong> GiaPhongs { get; set; } = new List<GiaPhong>();
        public virtual ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
    }
}
