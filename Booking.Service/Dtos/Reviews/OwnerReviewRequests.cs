using System.ComponentModel.DataAnnotations;

namespace Booking.Service.Dtos.Reviews
{
    public class OwnerReviewResponseRequest
    {
        [Required, StringLength(2000)]
        public string Response { get; set; } = string.Empty;
    }

    public class OwnerReviewReportRequest
    {
        [Required, StringLength(1000)]
        public string Reason { get; set; } = string.Empty;
    }
}