using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.KhachSans
{
    public interface IKhachSanRepository
    {
        Task<KhachSan> TaoKhachSan(KhachSan ks);
        Task<(List<KhachSan> Items, int TotalCount)> LayDanhSachKhachSanAdmin(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, int pageIndex, int pageSize);

        Task<(List<KhachSan> Items, int TotalCount)> LayDanhSachKhachSanOwner(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, Guid ownerId, int pageIndex, int pageSize);
        Task<(List<KhachSan> Items, int TotalCount)> FilterHotels(string? keyword, string provinceCode, int? soKhach, DateTime? ngayNhanPhong, DateTime? ngayTraPhong, int pageIndex, int pageSize);
        Task<KhachSan> DetailKhachSan(Guid id);
    }
}
