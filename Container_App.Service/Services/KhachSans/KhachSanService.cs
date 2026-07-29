using CloudinaryDotNet.Actions;
using Container_App.Common.Shared;
using Container_App.Common.Shared.Enum.Hotel;
using Container_App.Core.Model.KhachSanImage;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.LoaiPhongs;
using Container_App.Core.Model.TienIchs;
using Container_App.Data;
using Container_App.Data.Connection;
using Container_App.Data.Repository.GiaPhongs;
using Container_App.Data.Repository.KhachSanImage;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Redis;
using Container_App.Data.Repository.TienIchs;
using Container_App.Service.Dtos.KhachSan;
using Container_App.Service.Dtos.KhachSanDto;
using Container_App.Service.Dtos.KhachSanImages;
using Container_App.Service.Dtos.LoaiPhongs;
using Container_App.Service.Dtos.TienIchs;
using Container_App.Service.Services.Cloudinarys;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Printing;
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
        private readonly IGiaPhongRepository _giaPhongRepository;
        private readonly IRedisService _redisService;

        public KhachSanService(CloudinaryService cloudinaryService, IKhachSanRepository khachSanRepository,
            IKhachSanImageRepository khachSanImageRepository, IUnitOfWork unitOfWork, 
            IGiaPhongRepository giaPhongRepository,
            IRedisService redisService)
        {
            _cloudinaryService = cloudinaryService;
            _khachSanRepository = khachSanRepository;
            _khachSanImageRepository = khachSanImageRepository;
            _unitOfWork = unitOfWork;
            _giaPhongRepository = giaPhongRepository;
            _redisService = redisService;
        }

        public async Task<KhachSanDetailResponse?> DetailKhachSan(Guid id)
        {
            try
            {
                // 1. Lấy Entity từ Repository
                var khachSan = await _khachSanRepository.DetailKhachSan(id);
                if (khachSan == null)
                {
                    return null;
                }

                // 2. Map sang DTO tinh gọn tại Service
                return new KhachSanDetailResponse
                {
                    Id = khachSan.Id,
                    TenKhachSan = khachSan.TenKhachSan,
                    Mota = khachSan.MoTa,
                    DiaChi = khachSan.DiaChi,
                    SoSao = khachSan.SoSao,
                    GioNhanPhong = khachSan.GioNhanPhong,
                    GioTraPhong = khachSan.GioTraPhong,
                    TenThanhPho = khachSan.Province?.full_name ?? string.Empty,                  

                    // TienIchs bóc tách qua bảng trung gian
                    TienIchs = khachSan.KhachSan_TienIches
                        .Where(kst => kst.TienIch != null)
                        .Select(kst => new TienIchDto
                        {
                            Id = kst.TienIch!.Id,
                            TenTienIch = kst.TienIch.TenTienIch,
                            Icon = kst.TienIch.Icon
                        }).ToList(),

                    // KhachSanImages chỉ gồm Id và Url
                    KhachSanImages = khachSan.KhachSanImages.Select(img => new KhachSanImageDto
                    {
                        Id = img.Id,
                        Url = img.Url
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get detail KhachSan: {ex.Message}");
                return null;
            }
        }

        public async Task<FilterHotelResponseDto> FilterHotelsAsync(FilterHotelRequestDto dto)
        {
            var stopwatch = Stopwatch.StartNew();

            // 1. Tạo Cache Key chuẩn từ DTO
            string cacheKey = $"hotel-filter:" +
                              $"{dto.Keyword ?? ""}:" +
                              $"{dto.ProvinceCode ?? ""}:" +
                              $"{dto.SoKhach?.ToString() ?? ""}:" +
                              $"{dto.NgayNhanPhong?.ToString("yyyyMMdd") ?? ""}:" +
                              $"{dto.NgayTraPhong?.ToString("yyyyMMdd") ?? ""}:" +
                              $"{dto.Page}:" +
                              $"{dto.PageSize}";

            // 2. Kiểm tra Cache trong Redis
            var cachedData = await _redisService.GetObject<FilterHotelResponseDto>(cacheKey);
            if (cachedData != null)
            {
                stopwatch.Stop();
                Console.WriteLine($"[Service] Load từ Redis: {stopwatch.ElapsedMilliseconds} ms");
                return cachedData;
            }

            // 3. Gọi Repository lấy danh sách Khách sạn và Tổng số lượng dòng (TotalRow)
            var (khachSans, totalRow) = await _khachSanRepository.FilterHotels(
                dto.Keyword,
                dto.ProvinceCode,
                dto.SoKhach,
                dto.NgayNhanPhong,
                dto.NgayTraPhong,
                dto.Page,
                dto.PageSize);

            // Nếu không tìm thấy khách sạn nào, trả về đối tượng rỗng
            if (khachSans == null || !khachSans.Any())
            {
                return new FilterHotelResponseDto
                {
                    Data = new List<FilterHotelItemDto>(),
                    TotalRow = 0,
                    TotalPage = 0
                };
            }

            // 4. Lấy danh sách Ảnh theo HotelIds (Tránh lỗi N+1 Query)
            var hotelIds = khachSans.Select(x => x.Id).Distinct().ToList();
            var hotelImages = await _khachSanImageRepository.GetHotelImages(hotelIds);

            // Gom nhóm ảnh theo KhachSanId để Lookup nhanh với O(1)
            var imageLookup = hotelImages
                .GroupBy(x => x.KhachSanId)
                .ToDictionary(g => g.Key, g => g.Select(img => img.Url).ToList());


            var prices = await _giaPhongRepository.LayGiaPhongTheoDSKhachSanId(hotelIds);

            var priceLookup = prices.ToDictionary(
                x => x.LoaiPhong.KhachSanId!.Value,
                x => x.Gia);
            // 5. Map dữ liệu từ Entity -> FilterHotelItemDto
            var hotelDtos = khachSans.Select(hotel => new FilterHotelItemDto
            {
                Id = hotel.Id,
                TenKhachSan = hotel.TenKhachSan,
                MoTa = hotel.MoTa,
                DiaChi = hotel.DiaChi,
                SoSao = hotel.SoSao,
                Gia = priceLookup.TryGetValue(hotel.Id, out var gia) ? gia: 0,
                Urls = imageLookup.TryGetValue(hotel.Id, out var urls) ? urls : new List<string>()
            }).ToList();

            // 6. Tính tổng số trang bằng totalRow vừa nhận từ Repository
            int totalPage = Paginations.GetTotalPages(totalRow, dto.PageSize);

            var response = new FilterHotelResponseDto
            {
                Data = hotelDtos,
                TotalRow = totalRow,
                TotalPage = totalPage
            };

            // 7. Lưu vào Redis (Cache trong 30 giây)
            await _redisService.SetObject(cacheKey, response, TimeSpan.FromSeconds(30));

            stopwatch.Stop();
            Console.WriteLine($"[Service] FilterHotels DB Execution: {stopwatch.ElapsedMilliseconds} ms");

            return response;
        }



        // 1. Dành cho Admin: Lấy tất cả khách sạn theo bộ lọc
        public async Task<FilterHotelResponseDto> LayDanhSachKhachSanAdminAsync(AdminFilterHotelRequestDto dto)
        {
            var (khachSans, totalRow) = await _khachSanRepository.LayDanhSachKhachSanAdmin(
                dto.Keyword,
                dto.ThanhPho,
                dto.ViDo ?? 0,
                dto.KinhDo ?? 0,
                dto.SoSao,
                dto.TrangThai,
                dto.Page,
                dto.PageSize
            );

            return await MapToFilterHotelResponseDtoAsync(khachSans, totalRow, dto.PageSize);
        }

        // 2. Dành cho Owner: Lấy danh sách khách sạn do Owner đó sở hữu
        public async Task<FilterHotelResponseDto> LayDanhSachKhachSanOwnerAsync(OwnerFilterHotelRequestDto dto, Guid ownerId)
        {
            var (khachSans, totalRow) = await _khachSanRepository.LayDanhSachKhachSanOwner(
                dto.Keyword,
                dto.ThanhPho,
                dto.ViDo ?? 0,
                dto.KinhDo ?? 0,
                dto.SoSao,
                dto.TrangThai,
                ownerId,
                dto.Page,
                dto.PageSize
            );

            return await MapToFilterHotelResponseDtoAsync(khachSans, totalRow, dto.PageSize);
        }

        // Private Helper: Xử lý gom nhóm Ảnh, Mapping DTO và Phân trang để dùng chung
        private async Task<FilterHotelResponseDto> MapToFilterHotelResponseDtoAsync(
            List<KhachSan> khachSans,
            int totalRow,
            int pageSize)
        {
            // Nếu danh sách rỗng, trả về response trống lập tức
            if (khachSans == null || !khachSans.Any())
            {
                return new FilterHotelResponseDto
                {
                    Data = new List<FilterHotelItemDto>(),
                    TotalRow = 0,
                    TotalPage = 0
                };
            }

            // Lấy ảnh danh sách khách sạn theo HotelIds (Tối ưu performance)
            var hotelIds = khachSans.Select(x => x.Id).Distinct().ToList();
            var hotelImages = await _khachSanImageRepository.GetHotelImages(hotelIds);

            var imageLookup = hotelImages
                .GroupBy(x => x.KhachSanId)
                .ToDictionary(g => g.Key, g => g.Select(img => img.Url).ToList());

            // Map dữ liệu từ Entity sang DTO
            var hotelDtos = khachSans.Select(hotel => new FilterHotelItemDto
            {
                Id = hotel.Id,
                TenKhachSan = hotel.TenKhachSan,
                MoTa = hotel.MoTa,
                DiaChi = hotel.DiaChi,
                SoSao = hotel.SoSao,
                TrangThai = hotel.TrangThai,
                NgayTao = hotel.NgayTao,
                Urls = imageLookup.TryGetValue(hotel.Id, out var urls) ? urls : new List<string>()
            }).ToList();

            // Tính tổng số trang
            int totalPage = Paginations.GetTotalPages(totalRow, pageSize);

            return new FilterHotelResponseDto
            {
                Data = hotelDtos,
                TotalRow = totalRow,
                TotalPage = totalPage
            };
        }

        public async Task<KhachSanCreateResponse> TaoKhachSan(KhachSanCreateRequest ks, Guid nguoiTao)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (!TimeSpan.TryParse(ks.GioNhanPhong, out var gioNhan) ||
                    !TimeSpan.TryParse(ks.GioTraPhong, out var gioTra))
                {
                    return new KhachSanCreateResponse
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
                    TrangThai = TrangThaiKhachSan.CHO_DUYET.ToString(),
                    NguoiTao = nguoiTao,
                };
                await _khachSanRepository.TaoKhachSan(KhachSan);
                await _khachSanImageRepository.InsertKhachSanImage(khachSanId, images);
                await _unitOfWork.CommitAsync();
                return new KhachSanCreateResponse
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
                return new KhachSanCreateResponse
                {
                    status = false,
                    message = "Tạo khách sạn thất bại",
                    Data = null
                };
            }
        }
    }
}
