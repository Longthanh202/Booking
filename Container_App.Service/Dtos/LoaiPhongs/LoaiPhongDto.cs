using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.LoaiPhongs
{
    public class LoaiPhongDto
    {
        public Guid Id { get; set; }
        public string? TenLoaiPhong { get; set; } = string.Empty;
        public int? SoKhachToiDa { get; set; }
        public string? KieuGiuong { get; set; } = string.Empty;
        public string? MoTa { get; set; } = string.Empty;
        public DateTime? NgayTao { get; set; }
    }
}
