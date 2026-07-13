using Container_App.Core.Model.KhachSans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Interface.KhachSans
{
    public interface IKhachSanService
    {
        Task<int> TaoKhachSan(KhachSan ks);
        Task<IEnumerable<KhachSan>> LayDanhSachKhachSanAdmin(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, int startRow, int endRow);

        Task<IEnumerable<KhachSan>> LayDanhSachKhachSanOwner(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, Guid ownerId, int startRow, int endRow);
        Task<IEnumerable<KhachSan>> FilterHotels(string? keyword,int? provinceCode,int? soKhach,DateTime? ngayNhanPhong,DateTime? ngayTraPhong,int startRow, int endRow);
        Task<KhachSan> DetailKhachSan(Guid id);
    }
}
