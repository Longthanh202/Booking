using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Service.Dtos.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.LoaiPhongs
{
    public interface ILoaiPhongService
    {
        Task<LoaiPhong> TaoLoaiPhong(LoaiPhongRequest lp);
        Task<List<LoaiPhongHienThi>> GetLoaiPhongByKhachSanId(GetLoaiPhongDto input);
    }
}
