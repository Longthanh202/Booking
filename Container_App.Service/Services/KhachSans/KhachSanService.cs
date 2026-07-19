using CloudinaryDotNet.Actions;
using Container_App.Common.Shared;
using Container_App.Core.Model.KhachSanImage;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.TienIchs;
using Container_App.Data;
using Container_App.Data.Connection;
using Container_App.Data.Repository.KhachSanImage;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.TienIchs;
using Container_App.Service.Dtos.KhachSan;
using Container_App.Service.Dtos.KhachSanDto;
using Container_App.Service.Services.Cloudinarys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.KhachSans
{
    public class KhachSanService : IKhachSanService
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly IKhachSanRepository _khachSanRepository;
        private readonly IKhachSanImageRepository _khachSanImageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoaiPhongRepository _loaiPhongRepository;
        private readonly ITienIchRepository _tienIchRepository;

        public KhachSanService(CloudinaryService cloudinaryService, IKhachSanRepository khachSanRepository,
            IKhachSanImageRepository khachSanImageRepository, IUnitOfWork unitOfWork, 
            ILoaiPhongRepository loaiPhongRepository, ITienIchRepository tienIchRepository)
        {
            _cloudinaryService = cloudinaryService;
            _khachSanRepository = khachSanRepository;
            _khachSanImageRepository = khachSanImageRepository;
            _unitOfWork = unitOfWork;
            _loaiPhongRepository = loaiPhongRepository;
            _tienIchRepository = tienIchRepository;
        }

        public async Task<KhachSanDetailReponse?> DetailKhachSan(Guid id)
        {
            try
            {
                // 1. Lấy thực thể khách sạn (đã được Include sẵn các bảng con ở Repo)
                var khachSan = await _khachSanRepository.DetailKhachSan(id);
                if (khachSan == null)
                {
                    return null;
                }

                // 2. Trả về cấu trúc response và "lọc sạch" dữ liệu rác tại đây
                return new KhachSanDetailReponse
                {
                    Id = khachSan.Id,
                    TenKhachSan = khachSan.TenKhachSan,
                    Mota = khachSan.MoTa,
                    DiaChi = khachSan.DiaChi,
                    SoSao = khachSan.SoSao,
                    GioNhanPhong = khachSan.GioNhanPhong,
                    GioTraPhong = khachSan.GioTraPhong,
                    full_name = khachSan.Province?.full_name ?? "",

                    // Lọc dữ liệu sạch cho loaiPhongs (Triệt tiêu liên kết ngược gây rác JSON)
                    loaiPhongs = khachSan.LoaiPhongs.Select(lp => new LoaiPhong
                    {
                        Id = lp.Id,
                        TenLoaiPhong = lp.TenLoaiPhong,
                        SoKhachToiDa = lp.SoKhachToiDa,
                        KieuGiuong = lp.KieuGiuong,
                        MoTa = lp.MoTa,
                        NgayTao = lp.NgayTao,
                        KhachSanId = id,
                        KhachSan = null
                    }).ToList(),

                    // Lọc tiện ích: Đi qua bảng trung gian KhachSan_TienIches để bóc lấy đối tượng TienIch
                    tienIchs = khachSan.KhachSan_TienIches
                        .Where(kst => kst.TienIch != null) // Phòng trường hợp dữ liệu lỗi dưới DB
                        .Select(kst => new TienIch
                        {
                            Id = kst.TienIch.Id,
                            TenTienIch = kst.TienIch.TenTienIch,
                            Icon = kst.TienIch.Icon
                        }).ToList(),

                    // Lọc hình ảnh trực tiếp từ tập hợp KhachSanImages của thực thể khachSan
                    KhachSanImages = khachSan.KhachSanImages.Select(img => new KhachSanImages
                    {
                        Id = img.Id,
                        Url = img.Url,
                        KhachSanId = id,
                        KhachSan = null
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get detail KhachSan: {ex.Message}");
                return null;
            }
        }

        public Task<List<KhachSan>> FilterHotels(string? keyword, int? provinceCode, int? soKhach, DateTime? ngayNhanPhong, DateTime? ngayTraPhong, int startRow, int endRow)
        {
            throw new NotImplementedException();
        }

        public Task<List<KhachSan>> LayDanhSachKhachSanAdmin(string keyword, string thanhPho, double viDo, double kinhDo, int soSao, string trangThai, int startRow, int endRow)
        {
            throw new NotImplementedException();
        }

        public Task<List<KhachSan>> LayDanhSachKhachSanOwner(string keyword, string thanhPho, double viDo, double kinhDo, int soSao, string trangThai, Guid ownerId, int startRow, int endRow)
        {
            throw new NotImplementedException();
        }

        public async Task<KhachSanCreateReponse> TaoKhachSan(KhachSanCreateRequest ks, Guid nguoiTao)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (!TimeSpan.TryParse(ks.GioNhanPhong, out var gioNhan) ||
                    !TimeSpan.TryParse(ks.GioTraPhong, out var gioTra))
                {
                    return new KhachSanCreateReponse
                    {
                        status = false,
                        message = "Giờ nhận phòng hoặc giờ trả phòng không hợp lệ",
                        Data = null
                    };
                }    

                var images = new List<string>();
                foreach (var file in ks.Files)
                {
                    var url = await _cloudinaryService.UploadImageAsync(file);
                    images.Add(url);
                }
                Guid khachSanId = Guid.NewGuid();
                var KhachSan = new KhachSan
                {
                    Id = khachSanId,
                    TenKhachSan = ks.TenKhachSan,
                    MoTa = ks.MoTa,
                    DiaChi = ks.DiaChi,
                    ThanhPho = ks.ThanhPho,
                    ViDo = ks.ViDo,
                    KinhDo = ks.KinhDo,
                    SoSao = ks.SoSao,
                    GioNhanPhong = Convert.ToDateTime(ks.GioNhanPhong).TimeOfDay,
                    GioTraPhong = Convert.ToDateTime(ks.GioTraPhong).TimeOfDay,
                    TrangThai = ks.TrangThai,
                    NguoiTao = nguoiTao,
                };
                await _khachSanRepository.TaoKhachSan(KhachSan);
                await _khachSanImageRepository.InsertKhachSanImage(khachSanId, images);
                await _unitOfWork.CommitAsync();
                return new KhachSanCreateReponse
                {
                    status = true,
                    message = "Tạo khách sạn thành công",
                    Data = KhachSan
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Error when create KhachSan: {ex.Message}");
                return new KhachSanCreateReponse
                {
                    status = false,
                    message = "Tạo khách sạn thất bại",
                    Data = null
                };
            }
        }
    }
}
