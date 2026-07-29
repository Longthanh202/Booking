using Container_App.Core.Model.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.LoaiPhongs
{
    public interface ILoaiPhongRepository
    {
        Task<LoaiPhong> TaoLoaiPhong(LoaiPhong lp);
        Task<List<LoaiPhongHienThi>> GetLoaiPhongByKhachSanId(Guid khachSanId, 
            int soKhach, DateTime? ngayNhan, DateTime? ngayTra);
    }
}
