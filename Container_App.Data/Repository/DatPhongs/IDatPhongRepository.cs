using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.PhongDats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.DatPhongs
{
    public interface IDatPhongRepository
    {
        Task<DatPhong> DatPhong(DatPhong dp, List<ChiTietDatPhong> ctdp, List<PhongDat> phongDats);
        Task<DatPhong?> LayTheoId(Guid id);
        Task CapNhatTrangThai(DatPhong d);
    }
}
