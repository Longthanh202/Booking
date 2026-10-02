using Booking.Common.Shared.Enum.Payment;
using Booking.Core.Model.DatPhongs;
using Booking.Data.Repository.TaiKhoanNganHangs;
using Booking.Data.Repository.ThanhToans;
using Booking.Service.Dtos.Payments;
using System.Globalization;

namespace Booking.Service.Services.ThanhToans
{
    public class ThanhToanService : IThanhToanService
    {
        private readonly IThanhToanRepository _repository;
        private readonly ITaiKhoanNganHangRepository _bankRepository;

        public ThanhToanService(IThanhToanRepository repository, ITaiKhoanNganHangRepository bankRepository)
        {
            _repository = repository;
            _bankRepository = bankRepository;
        }

        public Task<ThanhToan?> GetForCustomer(Guid paymentId, Guid customerId) =>
            _repository.GetForCustomer(paymentId, customerId);

        public async Task<PaymentQrResponse?> GetQrForCustomerBooking(Guid bookingId, Guid customerId)
        {
            var payment = await _repository.GetForCustomerBooking(bookingId, customerId);
            var booking = payment?.DatPhong;
            var amount = payment?.SoTien;

            if (payment?.TrangThai != TrangThaiThanhToan.CHO_THANH_TOAN.ToString() ||
                booking?.KhachSan == null ||
                !amount.HasValue || amount.Value <= 0 || decimal.Truncate(amount.Value) != amount.Value)
            {
                return null;
            }

            var bank = (await _bankRepository.GetByUser(booking.KhachSan.NguoiTao))
                .Where(x => x.IsDefault)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();

            if (bank == null || string.IsNullOrWhiteSpace(bank.TenNganHang) ||
                string.IsNullOrWhiteSpace(bank.SoTaiKhoan) || string.IsNullOrWhiteSpace(bank.ChuTaiKhoan))
            {
                return null;
            }

            var amountText = amount.Value.ToString("0", CultureInfo.InvariantCulture);
            var bankCode = Uri.EscapeDataString(bank.TenNganHang.Trim());
            var accountNumber = Uri.EscapeDataString(bank.SoTaiKhoan.Trim());
            var accountName = Uri.EscapeDataString(bank.ChuTaiKhoan.Trim());
            var paymentNote = Uri.EscapeDataString($"{bookingId:N}"[..15]);
            var qrCodeUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact2.png" +
                $"?amount={amountText}&addInfo={paymentNote}&accountName={accountName}";

            return new PaymentQrResponse
            {
                BookingId = bookingId,
                PaymentId = payment.Id,
                Amount = amount.Value,
                BankCode = bank.TenNganHang,
                AccountNumber = bank.SoTaiKhoan,
                AccountName = bank.ChuTaiKhoan,
                PaymentNote = Uri.UnescapeDataString(paymentNote),
                QrCodeUrl = qrCodeUrl
            };
        }

        public Task<List<ThanhToan>> GetAll() => _repository.GetAll();

        public Task<ThanhToan?> UpdateStatus(Guid paymentId, UpdatePaymentStatusRequest request)
        {
            return !Enum.TryParse<TrangThaiThanhToan>(request.TrangThai, true, out var status)
                ? Task.FromResult<ThanhToan?>(null)
                : _repository.UpdateStatus(paymentId, status.ToString());
        }
    }
}