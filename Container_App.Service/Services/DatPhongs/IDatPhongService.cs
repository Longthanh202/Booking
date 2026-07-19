using Container_App.Core.Model.DatPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.DatPhongs
{
    public interface IDatPhongService
    {
        Task<DatPhong> DatPhong(DatPhong dp, List<ChiTietDatPhong> ctdp);
    }
}
