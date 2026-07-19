using Container_App.Core.Model.KhachSanImage;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.TienIchs;

namespace Container_App.Model.KhachSans
{
    public class DetailHotel
    {
        public KhachSan KhachSan { get; set; }
        public List<LoaiPhong> loaiPhongs { get; set; }
        public List<TienIch> tienIchs { get; set; }
        public List<KhachSanImages> KhachSanImages { get; set; }
    }
}
