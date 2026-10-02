using Booking.Common.Shared.Enum.Hotel;
using Booking.Core.Model.Phongs;
using Booking.Data.Connection;
using Booking.Data.Repository.Phongs;
using Booking.Data.Repository.LoaiPhongs;
using Booking.Service.Dtos.Rooms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Phongs
{
    public class PhongService : IPhongService
    {
        private readonly IPhongRepository _phongRepository;
        private readonly ILoaiPhongRepository _loaiPhongRepository;
        public PhongService(IPhongRepository phongRepository, ILoaiPhongRepository loaiPhongRepository)
        {
           _phongRepository = phongRepository;
           _loaiPhongRepository = loaiPhongRepository;
        }
        public async Task<Phong?> CreateRoom(CreateRoomRequest p, Guid ownerId)
        {
            if (p.LoaiPhongId == Guid.Empty ||
                await _loaiPhongRepository.GetRoomTypeForOwner(p.LoaiPhongId, ownerId) == null)
            {
                return null;
            }

            Phong phong = new Phong
            {
                Id = Guid.NewGuid(),
                LoaiPhongId = p.LoaiPhongId,
                SoPhong = p.SoPhong,
                Tang = p.Tang,
                TrangThai = string.IsNullOrWhiteSpace(p.TrangThai)
                    ? TrangThaiPhong.SAN_SANG.ToString()
                    : p.TrangThai,
            };
            return await _phongRepository.CreateRoom(phong);
        }

        public Task<List<Phong>> GetRooms(Guid roomTypeId, Guid ownerId) =>
            _phongRepository.GetRoomsByRoomType(roomTypeId, ownerId);

        public async Task<Phong?> UpdateRoom(Guid roomId, UpdateRoomRequest request, Guid ownerId)
        {
            var room = await _phongRepository.GetRoomForOwner(roomId, ownerId);
            if (room == null || string.IsNullOrWhiteSpace(request.SoPhong))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.TrangThai) &&
                !Enum.TryParse<TrangThaiPhong>(request.TrangThai, true, out _))
            {
                return null;
            }

            room.SoPhong = request.SoPhong.Trim();
            room.Tang = request.Tang;
            if (!string.IsNullOrWhiteSpace(request.TrangThai))
            {
                room.TrangThai = request.TrangThai.Trim().ToUpperInvariant();
            }
            return await _phongRepository.UpdateRoom(room);
        }

        public Task<bool> DeleteRoom(Guid roomId, Guid ownerId) =>
            _phongRepository.DeleteRoom(roomId, ownerId);
    }
}
