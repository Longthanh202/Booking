using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.DatPhongs
{
    public class DatPhongRequest
    {
        public Guid KhachSanId { get; set; }
        public DateTime? NgayNhanPhong { get; set; }
        public DateTime? NgayTraPhong { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public List<LoaiPhongDto>? DanhSachPhong { get; set; }
    }

    public class LoaiPhongDto
    {
        public Guid LoaiPhongId { get; set; }
        public int SoLuong { get; set; }
    }
}
