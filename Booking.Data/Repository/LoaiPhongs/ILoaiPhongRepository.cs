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
        Task<bool> CreateRoomType(LoaiPhong lp);
        Task<List<LoaiPhongHienThi>> GetRoomTypesByHotelId(Guid khachSanId,
            int soKhach, DateTime? ngayNhan, DateTime? ngayTra);

        Task<List<LoaiPhong>> GetOwnerRoomTypes(Guid khachSanId);
    }
}
