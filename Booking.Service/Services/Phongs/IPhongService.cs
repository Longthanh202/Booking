using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.Phongs;
using Booking.Service.Dtos.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Phongs
{
    public interface IPhongService
    {
        Task<Phong?> CreateRoom(CreateRoomRequest p, Guid ownerId);
        Task<List<Phong>> GetRooms(Guid roomTypeId, Guid ownerId);
        Task<Phong?> UpdateRoom(Guid roomId, UpdateRoomRequest request, Guid ownerId);
        Task<bool> DeleteRoom(Guid roomId, Guid ownerId);
    }
}
