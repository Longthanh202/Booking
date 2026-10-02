namespace Booking.Service.Dtos.Reviews
{
    public class CreateReviewRequest
    {
        public Guid KhachSanId { get; set; }
        public int SoSao { get; set; }
        public string NoiDung { get; set; } = string.Empty;
    }
}