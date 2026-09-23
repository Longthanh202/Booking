using Booking.Core.Model.DanhGias;
using Booking.Service.Dtos.Reviews;

namespace Booking.Service.Services.DanhGias
{
    public interface IDanhGiaService
    {
        Task<DanhGia?> Create(CreateReviewRequest request, Guid customerId);
        Task<List<DanhGia>> GetByHotel(Guid hotelId);
        Task<List<DanhGia>> GetByCustomer(Guid customerId);
        Task<List<DanhGia>?> GetByHotelForOwner(Guid hotelId, Guid ownerId);
        Task<bool> Delete(Guid reviewId);
    }
}