using System.ComponentModel.DataAnnotations;

namespace Booking.Core.Model.Phongs
{
    public class RoomAvailabilityBlock
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string? Reason { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Phong Room { get; set; } = null!;
    }
}