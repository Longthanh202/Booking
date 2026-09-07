using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.GiaPhongs;
using Booking.Core.Model.KhachSans;
using Booking.Core.Model.Phongs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.LoaiPhongs
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
