namespace Booking.Service.Dtos.Finance
{
    public class CreateWithdrawalRequest
    {
        public Guid KhachSanId { get; set; }
        public Guid TaiKhoanNganHangId { get; set; }
        public decimal SoTien { get; set; }
        public string? GhiChu { get; set; }
    }
}