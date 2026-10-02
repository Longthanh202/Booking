using Booking.Core.Model.DanhGias;

namespace Booking.Data.Repository.DanhGias
{
    public interface IDanhGiaRepository
    {
        Task<DanhGia?> Create(DanhGia review);
        Task<List<DanhGia>> GetByHotel(Guid hotelId);
        Task<List<DanhGia>> GetByCustomer(Guid customerId);
        Task<bool> CanReview(Guid hotelId, Guid customerId);
        Task<bool> IsHotelOwnedBy(Guid hotelId, Guid ownerId);
        Task<bool> RespondByOwner(Guid reviewId, Guid ownerId, string response);
        Task<bool> ReportByOwner(Guid reviewId, Guid ownerId, string reason);
        Task<bool> Delete(Guid reviewId);
    }
}