using System.ComponentModel.DataAnnotations;

namespace Booking.Core.Model.Tags
{
    public class TheTag
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

        public virtual ICollection<KhachSan_TheTag> KhachSan_TheTags { get; set; } = new List<KhachSan_TheTag>();
    }
}