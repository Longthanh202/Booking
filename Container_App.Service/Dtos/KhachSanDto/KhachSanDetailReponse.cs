using Container_App.Core.Model.KhachSanImage;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.TienIchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.KhachSanDto
{
    public class KhachSanDetailReponse
    {
        public Guid Id { get; set; }
        public string TenKhachSan { get; set; }
        public string Mota { get; set; }
        public string DiaChi { get; set; }
        public int SoSao { get; set; }
        public TimeSpan? GioNhanPhong { get; set; }
        public TimeSpan? GioTraPhong { get; set; }
        public string full_name { get; set; }
        public List<LoaiPhong> loaiPhongs { get; set; }
        public List<TienIch> tienIchs { get; set; }
        public List<KhachSanImages> KhachSanImages { get; set; }
    }
}
