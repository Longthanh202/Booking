using Booking.Common.Shared.Enum.Payment;
using Booking.Core.Model.DatPhongs;
using Booking.Data.Repository.ThanhToans;
using Booking.Service.Dtos.Payments;

namespace Booking.Service.Services.ThanhToans
{
    public class ThanhToanService : IThanhToanService
    {
        private readonly IThanhToanRepository _repository;

        public ThanhToanService(IThanhToanRepository repository)
        {
            _repository = repository;
        }

        public Task<ThanhToan?> GetForCustomer(Guid paymentId, Guid customerId) =>
            _repository.GetForCustomer(paymentId, customerId);

        public Task<List<ThanhToan>> GetAll() => _repository.GetAll();

        public Task<ThanhToan?> UpdateStatus(Guid paymentId, UpdatePaymentStatusRequest request)
        {
            return !Enum.TryParse<TrangThaiThanhToan>(request.TrangThai, true, out var status)
                ? Task.FromResult<ThanhToan?>(null)
                : _repository.UpdateStatus(paymentId, status.ToString());
        }
    }
}