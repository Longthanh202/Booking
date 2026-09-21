using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LoaiPhongs;
using Booking.Service.Dtos.LoaiPhongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.LoaiPhongs
{
    public interface ILoaiPhongService
    {
        Task<int> CreateRoomTypes(List<LoaiPhongRequest> lp);
        Task<List<LoaiPhongHienThi>> GetRoomTypesByHotelId(GetLoaiPhongDto input);
        Task<List<LoaiPhong>> GetOwnerRoomTypes(Guid khachSanId);
    }
}
