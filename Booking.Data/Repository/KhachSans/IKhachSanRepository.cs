using Booking.Core.Model.KhachSans;

namespace Booking.Data.Repository.KhachSans
{
    public interface IKhachSanRepository
    {
        Task<KhachSan> CreateHotel(KhachSan ks);
        Task<(List<KhachSan> Items, int TotalCount)> GetHotelsForAdmin(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, int pageIndex, int pageSize);

        Task<(List<KhachSan> Items, int TotalCount)> GetHotelsForOwner(Guid ownerId, int pageIndex, int pageSize);
        Task<(List<KhachSan> Items, int TotalCount)> FilterHotels(string? keyword, int? soKhach, DateTime? ngayNhanPhong, DateTime? ngayTraPhong, int pageIndex, int pageSize);
        Task<KhachSan?> GetHotelDetails(Guid id);
    }
}
