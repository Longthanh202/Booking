using Booking.Core.Model.GiaPhongs;
using Booking.Data.Repository.GiaPhongs;
using Booking.Service.Dtos.Rooms;

namespace Booking.Service.Services.GiaPhongs
{
    public class GiaPhongService : IGiaPhongService
    {
        private readonly IGiaPhongRepository _repository;

        public GiaPhongService(IGiaPhongRepository repository)
        {
            _repository = repository;
        }

        public async Task<GiaPhong?> CreatePrice(CreateRoomPriceRequest request, Guid ownerId)
        {
            if (request.LoaiPhongId == Guid.Empty || request.Gia <= 0 ||
                request.NgayBatDau == default ||
                (request.NgayKetThuc.HasValue && request.NgayKetThuc <= request.NgayBatDau))
            {
                return null;
            }

            return await _repository.CreatePrice(new GiaPhong
            {
                Id = Guid.NewGuid(),
                LoaiPhongId = request.LoaiPhongId,
                Gia = request.Gia,
                NgayBatDau = request.NgayBatDau,
                NgayKetThuc = request.NgayKetThuc,
                IsActive = true,
                NgayTao = DateTime.UtcNow
            }, ownerId);
        }

        public Task<List<GiaPhong>> GetPrices(Guid roomTypeId, Guid ownerId) =>
            _repository.GetPricesByRoomType(roomTypeId, ownerId);
    }
}