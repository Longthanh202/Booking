using Container_App.Core.Model.KhachSanImage;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.TienIchs;
using Container_App.Service.Dtos.KhachSanImages;
using Container_App.Service.Dtos.LoaiPhongs;
using Container_App.Service.Dtos.TienIchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.KhachSanDto
{
    public class KhachSanDetailResponse
    {
        public Guid Id { get; set; }
        public string? TenKhachSan { get; set; } = string.Empty;
        public string? Mota { get; set; } = string.Empty;
        public string? DiaChi { get; set; } = string.Empty;
        public int? SoSao { get; set; }
        public TimeSpan? GioNhanPhong { get; set; }
        public TimeSpan? GioTraPhong { get; set; }
        public string? TenThanhPho { get; set; } = string.Empty; // Đổi tên rõ nghĩa hơn full_name     
        public List<TienIchDto> TienIchs { get; set; } = new();
        public List<KhachSanImageDto> KhachSanImages { get; set; } = new();
    }
}
