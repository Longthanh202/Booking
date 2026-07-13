using Container_App.Core.Model.KhachSans;

namespace Container_App.Model.KhachSans
{
    public class FilterHotelResponse
    {
        public List<KhachSan> Data { get; set; } = new();

        public int TotalPage { get; set; }
    }
}
