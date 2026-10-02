namespace Booking.Service.Dtos.Authentication
{
    public class ResetPasswordRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}