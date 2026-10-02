using Booking.Core.Model.TaiKhoanNganHangs;

namespace Booking.Data.Repository.TaiKhoanNganHangs
{
    public interface ITaiKhoanNganHangRepository
    {
        Task<List<TaiKhoanNganHang>> GetByUser(Guid userId);
        Task<TaiKhoanNganHang?> GetOwned(Guid id, Guid userId);
        Task<TaiKhoanNganHang> Create(TaiKhoanNganHang account);
    }
}