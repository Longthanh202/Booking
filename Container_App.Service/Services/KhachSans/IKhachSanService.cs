using Container_App.Core.Model.KhachSans;
using Container_App.Service.Dtos.KhachSan;
using Container_App.Service.Dtos.KhachSanDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.KhachSans
{
    public interface IKhachSanService
    {
        Task<KhachSanCreateReponse> TaoKhachSan(KhachSanCreateRequest ks, Guid nguoiTao);
        Task<List<KhachSan>> LayDanhSachKhachSanAdmin(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, int startRow, int endRow);

        Task<List<KhachSan>> LayDanhSachKhachSanOwner(string keyword, string thanhPho,
            double viDo, double kinhDo, int soSao, string trangThai, Guid ownerId, int startRow, int endRow);
        Task<List<KhachSan>> FilterHotels(string? keyword,int? provinceCode,int? soKhach,DateTime? ngayNhanPhong,DateTime? ngayTraPhong,int startRow, int endRow);
        Task<KhachSanDetailReponse> DetailKhachSan(Guid id);
    }
}
