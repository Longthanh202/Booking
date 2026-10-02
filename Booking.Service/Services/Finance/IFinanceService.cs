using Booking.Core.Model.ChiTraKhachSans;
using Booking.Core.Model.LichSuVis;
using Booking.Core.Model.TaiKhoanNganHangs;
using Booking.Core.Model.ViKhachSans;
using Booking.Service.Dtos.Finance;

namespace Booking.Service.Services.Finance
{
    public interface IFinanceService
    {
        Task<List<ViKhachSan>> GetWallets(Guid ownerId);
        Task<List<LichSuVi>> GetStatement(Guid ownerId, Guid? khachSanId, DateTime? from, DateTime? to);
        Task<List<TaiKhoanNganHang>> GetBankAccounts(Guid ownerId);
        Task<TaiKhoanNganHang?> AddBankAccount(Guid ownerId, CreateBankAccountRequest request);
        Task<ChiTraKhachSan?> RequestWithdrawal(Guid ownerId, CreateWithdrawalRequest request);
        Task<List<ChiTraKhachSan>> GetWithdrawals(Guid ownerId);
        Task<List<ChiTraKhachSan>> GetPendingWithdrawals();
        Task<ChiTraKhachSan?> UpdateWithdrawal(long id, UpdatePayoutRequest request);
    }
}