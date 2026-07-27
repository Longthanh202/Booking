using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.Phongs
{
    public class PhongRequest
    {
        public Guid LoaiPhongId { get; set; }
        public string? SoPhong { get; set; }
        public int? Tang { get; set; }
        public string? TrangThai { get; set; }
    }
}
