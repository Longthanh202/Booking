using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.KhachSanImage;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.Provinces;
using Container_App.Core.Model.TienIchs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.KhachSans
{
    public class KhachSan
    {
        [Key]
        public Guid Id { get; set; }
        public Guid NguoiTao { get; set; }
        public string TenKhachSan { get; set; }
        public string MoTa { get; set; }
        public string DiaChi { get; set; }
        public string ThanhPho { get; set; }
        public double? ViDo { get; set; }
        public double? KinhDo { get; set; }
        public int SoSao { get; set; }
        public TimeSpan? GioNhanPhong { get; set; }
        public TimeSpan? GioTraPhong { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public Province Province { get; set; }
        public ICollection<KhachSanImages> KhachSanImages { get; set; } 
            = new List<KhachSanImages>();   
        public ICollection<LoaiPhong> LoaiPhongs { get; set; } 
            = new List<LoaiPhong>();
        public ICollection<DatPhong> datPhongs { get; set; } 
            = new List<DatPhong>();
        public ICollection<KhachSan_TienIch> KhachSan_TienIches { get; set; } 
            = new List<KhachSan_TienIch>();
    }
}
