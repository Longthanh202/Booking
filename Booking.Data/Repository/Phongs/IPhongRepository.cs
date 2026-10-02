using Booking.Core.Model.Phongs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repository.Phongs
{
    public interface IPhongRepository
    {
        Task<Phong> CreateRoom(Phong p);
        Task<Phong?> GetRoomForOwner(Guid roomId, Guid ownerId);
        Task<List<Phong>> GetRoomsByRoomType(Guid roomTypeId, Guid ownerId);
        Task<Phong?> UpdateRoom(Phong p);
        Task<bool> DeleteRoom(Guid roomId, Guid ownerId);
        Task<List<Phong>> LayDanhSachPhongTrong(Guid loaiPhongId, DateTime? ngayNhan, DateTime? ngayTra);
    }
}
