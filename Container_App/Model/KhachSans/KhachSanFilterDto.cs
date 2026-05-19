namespace Container_App.Model.KhachSans
{
    public class KhachSanFilterDto
    {
        public string? Keyword { get; set; }
        public string? ThanhPho { get; set; }
        public float? ViDo { get; set; }
        public float? KinhDo { get; set; }
        public int SoSao { get; set; }
        public string? TrangThai { get; set; }
        public int Page { get; set; }
    }
    public class FilterHotelsDto : KhachSanFilterDto
    {
        public int? ProvinceCode { get; set; }

        public int? SoKhach { get; set; }

        public DateTime? NgayNhanPhong { get; set; }

        public DateTime? NgayTraPhong { get; set; }
    }
}
