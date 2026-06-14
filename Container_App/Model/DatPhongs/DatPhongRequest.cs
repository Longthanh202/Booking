using Container_App.Core.Model.DatPhongs;

namespace Container_App.Model.DatPhongs
{
    public class DatPhongRequest
    {
        public DatPhong DatPhong { get; set; }

        public List<ChiTietDatPhong> ChiTietDatPhongs
        {
            get; set;
        }
    }
}
