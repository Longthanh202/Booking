namespace Booking.Service.Dtos.Finance
{
    public class UpdatePayoutRequest
    {
        public string TrangThai { get; set; } = string.Empty;
        public string? MaGiaoDich { get; set; }
        public string? GhiChu { get; set; }
    }
}