using Booking.Core.Model.DanhGias;
using Booking.Data.Repository.DanhGias;
using Booking.Service.Dtos.Reviews;

namespace Booking.Service.Services.DanhGias
{
    public class DanhGiaService : IDanhGiaService
    {
        private readonly IDanhGiaRepository _repository;

        public DanhGiaService(IDanhGiaRepository repository)
        {
            _repository = repository;
        }

        public async Task<DanhGia?> Create(CreateReviewRequest request, Guid customerId)
        {
            if (request.KhachSanId == Guid.Empty || request.SoSao < 1 || request.SoSao > 5 ||
                string.IsNullOrWhiteSpace(request.NoiDung) ||
                !await _repository.CanReview(request.KhachSanId, customerId))
            {
                return null;
            }

            return await _repository.Create(new DanhGia
            {
                Id = Guid.NewGuid(),
                KhachSanId = request.KhachSanId,
                KhachHangId = customerId,
                SoSao = request.SoSao,
                NoiDung = request.NoiDung.Trim(),
                NgayTao = DateTime.UtcNow
            });
        }

        public Task<List<DanhGia>> GetByHotel(Guid hotelId) => _repository.GetByHotel(hotelId);

        public Task<List<DanhGia>> GetByCustomer(Guid customerId) => _repository.GetByCustomer(customerId);

        public async Task<List<DanhGia>?> GetByHotelForOwner(Guid hotelId, Guid ownerId)
        {
            return await _repository.IsHotelOwnedBy(hotelId, ownerId)
                ? await _repository.GetByHotel(hotelId)
                : null;
        }

        public Task<bool> Delete(Guid reviewId) => _repository.Delete(reviewId);
    }
}