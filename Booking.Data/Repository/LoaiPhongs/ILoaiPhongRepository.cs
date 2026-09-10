using Booking.Core.Model.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.LoaiPhongs
{
    public interface ILoaiPhongRepository
    {
        Task<bool> TaoLoaiPhong(LoaiPhong lp);
        Task<List<LoaiPhongHienThi>> GetLoaiPhongByKhachSanId(Guid khachSanId, 
            int soKhach, DateTime? ngayNhan, DateTime? ngayTra);

        Task<List<LoaiPhong>> GetLoaiPhongOwner(Guid khachSanId);
    }
}
