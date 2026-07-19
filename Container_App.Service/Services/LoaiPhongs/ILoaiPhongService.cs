using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.LoaiPhongs
{
    public interface ILoaiPhongService
    {
        Task<int> TaoLoaiPhong(LoaiPhong lp);
        Task<IEnumerable<LoaiPhong>> GetLoaiPhongByKhachSanId(Guid khachSanId);
    }
}
