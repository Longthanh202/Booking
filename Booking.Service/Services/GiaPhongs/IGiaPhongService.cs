using Booking.Core.Model.GiaPhongs;
using Booking.Service.Dtos.Rooms;

namespace Booking.Service.Services.GiaPhongs
{
    public interface IGiaPhongService
    {
        Task<GiaPhong?> CreatePrice(CreateRoomPriceRequest request, Guid ownerId);
        Task<List<GiaPhong>> GetPrices(Guid roomTypeId, Guid ownerId);
    }
}