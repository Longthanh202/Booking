namespace Booking.Service.Dtos.Tags
{
    public class TagResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public int HotelCount { get; set; }
    }

    public class TagHotelResponse
    {
        public Guid Id { get; set; }
        public string? TenKhachSan { get; set; }
        public string? MoTa { get; set; }
        public string? DiaChi { get; set; }
        public string? ThanhPho { get; set; }
        public int? SoSao { get; set; }
        public string? TrangThai { get; set; }
        public List<string> Urls { get; set; } = new();
    }

    public class TagHotelsPageResponse
    {
        public TagResponse Tag { get; set; } = new();
        public List<TagHotelResponse> Data { get; set; } = new();
        public int TotalRow { get; set; }
        public int TotalPage { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}