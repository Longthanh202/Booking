using System.ComponentModel.DataAnnotations;

namespace Booking.Core.Model.Tags
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<HotelTag> HotelTags { get; set; } = new List<HotelTag>();
    }
}