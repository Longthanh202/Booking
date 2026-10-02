namespace Booking.Service.Dtos.Finance
{
    public class CreateBankAccountRequest
    {
        public string TenNganHang { get; set; } = string.Empty;
        public string SoTaiKhoan { get; set; } = string.Empty;
        public string ChuTaiKhoan { get; set; } = string.Empty;
        public string? ChiNhanh { get; set; }
        public string? QrCode { get; set; }
        public bool IsDefault { get; set; } = true;
    }
}