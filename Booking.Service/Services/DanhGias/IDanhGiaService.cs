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
        Task<bool> RespondByOwner(Guid reviewId, Guid ownerId, string response);
        Task<bool> ReportByOwner(Guid reviewId, Guid ownerId, string reason);
        Task<bool> Delete(Guid reviewId);
    }
}