using Booking.Core.Model.KhachSans;

namespace Booking.Data.Repository.KhachSans
{
    public interface IKhachSanRepository
    {
        Task<KhachSan> TaoKhachSan(KhachSan ks);
        Task<(List<KhachSan> Items, int TotalCount)> LayDanhSachKhachSanAdmin(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, int pageIndex, int pageSize);

        Task<(List<KhachSan> Items, int TotalCount)> LayDanhSachKhachSanOwner(Guid ownerId, int pageIndex, int pageSize);
        Task<(List<KhachSan> Items, int TotalCount)> FilterHotels(string? keyword, int? soKhach, DateTime? ngayNhanPhong, DateTime? ngayTraPhong, int pageIndex, int pageSize);
        Task<KhachSan?> DetailKhachSan(Guid id);
    }
}
