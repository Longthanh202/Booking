using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LoaiPhongs;
using Booking.Service.Dtos.RoomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.LoaiPhongs
{
    public interface ILoaiPhongService
    {
        Task<int> CreateRoomTypes(List<CreateRoomTypeRequest> lp);
        Task<List<LoaiPhongHienThi>> GetRoomTypesByHotelId(RoomTypeSearchRequest input);
        Task<List<LoaiPhong>> GetOwnerRoomTypes(Guid khachSanId);
    }
}
