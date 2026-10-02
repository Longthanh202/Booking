namespace Booking.Service.Dtos.Payments
{
    public class PaymentQrResponse
    {
        public Guid BookingId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string PaymentNote { get; set; } = string.Empty;
        public string QrCodeUrl { get; set; } = string.Empty;
    }
}