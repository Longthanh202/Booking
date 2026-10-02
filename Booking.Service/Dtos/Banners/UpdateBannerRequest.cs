namespace Booking.Service.Dtos.Banners
{
    public class UpdateBannerRequest
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Url { get; set; }
        public int IsActive { get; set; }
    }
}