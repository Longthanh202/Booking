using Booking.Common.Shared.Enum.Payment;
using Booking.Common.Shared.Enum.Wallet;
using Booking.Core.Model.ChiTraKhachSans;
using Booking.Core.Model.LichSuVis;
using Booking.Core.Model.TaiKhoanNganHangs;
using Booking.Core.Model.ViKhachSans;
using Booking.Data;
using Booking.Data.Repository.ChiTraKhachSans;
using Booking.Data.Repository.LichSuVis;
using Booking.Data.Repository.TaiKhoanNganHangs;
using Booking.Data.Repository.ViKhachSans;
using Booking.Service.Dtos.Finance;

namespace Booking.Service.Services.Finance
{
    public class FinanceService : IFinanceService
    {
        private readonly IViKhachSanRepository _wallets;
        private readonly ILichSuViRepository _statements;
        private readonly IChiTraKhachSanRepository _payouts;
        private readonly ITaiKhoanNganHangRepository _banks;
        private readonly IUnitOfWork _unitOfWork;

        public FinanceService(IViKhachSanRepository wallets, ILichSuViRepository statements,
            IChiTraKhachSanRepository payouts, ITaiKhoanNganHangRepository banks, IUnitOfWork unitOfWork)
        {
            _wallets = wallets;
            _statements = statements;
            _payouts = payouts;
            _banks = banks;
            _unitOfWork = unitOfWork;
        }

        public Task<List<ViKhachSan>> GetWallets(Guid ownerId) => _wallets.LayTheoOwner(ownerId);

        public async Task<List<LichSuVi>> GetStatement(Guid ownerId, Guid? khachSanId, DateTime? from, DateTime? to)
        {
            var wallets = await _wallets.LayTheoOwner(ownerId);
            if (khachSanId.HasValue)
            {
                wallets = wallets.Where(x => x.MaKhachSan == khachSanId.Value).ToList();
            }

            var result = new List<LichSuVi>();
            foreach (var wallet in wallets)
            {
                var rows = await _statements.LayTheoVi(wallet.MaVi);
                result.AddRange(rows.Where(x => (!from.HasValue || x.NgayTao >= from.Value) &&
                    (!to.HasValue || x.NgayTao < to.Value.Date.AddDays(1))));
            }

            return result.OrderByDescending(x => x.NgayTao).ToList();
        }

        public Task<List<TaiKhoanNganHang>> GetBankAccounts(Guid ownerId) => _banks.GetByUser(ownerId);

        public async Task<TaiKhoanNganHang?> AddBankAccount(Guid ownerId, CreateBankAccountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TenNganHang) || string.IsNullOrWhiteSpace(request.SoTaiKhoan) ||
                string.IsNullOrWhiteSpace(request.ChuTaiKhoan))
            {
                return null;
            }

            return await _banks.Create(new TaiKhoanNganHang
            {
                Id = Guid.NewGuid(), UserId = ownerId, TenNganHang = request.TenNganHang.Trim(),
                SoTaiKhoan = request.SoTaiKhoan.Trim(), ChuTaiKhoan = request.ChuTaiKhoan.Trim(),
                ChiNhanh = request.ChiNhanh, QrCode = request.QrCode, IsDefault = request.IsDefault,
                CreatedDate = DateTime.UtcNow
            });
        }

        public async Task<ChiTraKhachSan?> RequestWithdrawal(Guid ownerId, CreateWithdrawalRequest request)
        {
            if (request.SoTien <= 0)
            {
                return null;
            }

            var wallet = await _wallets.LayTheoKhachSanCuaOwner(request.KhachSanId, ownerId);
            var bank = await _banks.GetOwned(request.TaiKhoanNganHangId, ownerId);
            if (wallet == null || bank == null || wallet.SoDu < request.SoTien)
            {
                return null;
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                wallet.SoDu -= request.SoTien;
                wallet.SoDuTamGiu += request.SoTien;
                await _wallets.CapNhat(wallet);
                var payout = new ChiTraKhachSan
                {
                    KhachSanId = request.KhachSanId, TaiKhoanNganHangId = request.TaiKhoanNganHangId,
                    SoTien = request.SoTien, TrangThai = TrangThaiChiTraKhachSan.CHO_CHI_TRA.ToString(),
                    GhiChu = request.GhiChu, NgayTao = DateTime.UtcNow
                };
                await _payouts.Tao(payout);
                await _statements.Tao(new LichSuVi
                {
                    MaVi = wallet.MaVi, SoTien = request.SoTien,
                    LoaiGiaoDich = LoaiGiaoDichVi.RUT_TIEN.ToString(),
                    NoiDung = "Tạo yêu cầu rút tiền", NgayTao = DateTime.UtcNow
                });
                await _unitOfWork.CommitAsync();
                return payout;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ChiTraKhachSan>> GetWithdrawals(Guid ownerId) =>
            (await _payouts.LayTheoOwner(ownerId)).ToList();

        public async Task<List<ChiTraKhachSan>> GetPendingWithdrawals() =>
            (await _payouts.LayChoChiTra()).ToList();

        public async Task<ChiTraKhachSan?> UpdateWithdrawal(long id, UpdatePayoutRequest request)
        {
            if (!Enum.TryParse<TrangThaiChiTraKhachSan>(request.TrangThai, true, out var status))
            {
                return null;
            }

            var payout = await _payouts.LayTheoId(id);
            if (payout == null)
            {
                return null;
            }

            if (payout.TrangThai == TrangThaiChiTraKhachSan.DA_CHI_TRA.ToString() ||
                payout.TrangThai == TrangThaiChiTraKhachSan.THAT_BAI.ToString() ||
                payout.TrangThai == TrangThaiChiTraKhachSan.DA_HUY.ToString())
            {
                return null;
            }

            var wallet = await _wallets.LayTheoKhachSan(payout.KhachSanId);
            if (wallet == null || wallet.SoDuTamGiu < payout.SoTien)
            {
                return null;
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (status == TrangThaiChiTraKhachSan.DA_CHI_TRA)
                {
                    wallet.SoDuTamGiu -= payout.SoTien;
                }
                else if (status == TrangThaiChiTraKhachSan.THAT_BAI || status == TrangThaiChiTraKhachSan.DA_HUY)
                {
                    wallet.SoDuTamGiu -= payout.SoTien;
                    wallet.SoDu += payout.SoTien;
                }

                await _wallets.CapNhat(wallet);
                payout.TrangThai = status.ToString();
                payout.MaGiaoDich = request.MaGiaoDich;
                payout.GhiChu = request.GhiChu ?? payout.GhiChu;
                payout.NgayChiTra = status == TrangThaiChiTraKhachSan.DA_CHI_TRA ? DateTime.UtcNow : payout.NgayChiTra;
                await _payouts.CapNhat(payout);
                await _unitOfWork.CommitAsync();
                return payout;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}