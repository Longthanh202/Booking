using System.ComponentModel.DataAnnotations;

namespace Booking.Service.Dtos.Tags
{
    public class SaveTagRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SetTagHotelsRequest
    {
        public List<Guid> HotelIds { get; set; } = new();
    }
}