using CloudinaryDotNet.Actions;
using Booking.Common.Shared;
using Booking.Common.Shared.Enum.Hotel;
using Booking.Core.Model.KhachSanImage;
using Booking.Core.Model.KhachSans;
using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.TienIchs;
using Booking.Data;
using Booking.Data.Connection;
using Booking.Data.Repository.GiaPhongs;
using Booking.Data.Repository.KhachSanImage;
using Booking.Data.Repository.KhachSans;
using Booking.Data.Repository.Redis;
using Booking.Service.Dtos.Hotels;
using Booking.Service.Dtos.Common;
using Booking.Service.Dtos.HotelImages;
using Booking.Service.Dtos.Amenities;
using Booking.Service.Services.Cloudinarys;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Booking.Service.Services.KhachSans
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

        public async Task<HotelDetailsResponse?> GetHotelDetails(Guid id)
        {
            // 1. Lấy Entity từ Repository
            var khachSan = await _khachSanRepository.GetHotelDetails(id);
            if (khachSan == null)
            {
                return null;
            }

            // 2. Map sang DTO tinh gọn tại Service
            return new HotelDetailsResponse
            {
                Id = khachSan.Id,
                TenKhachSan = khachSan.TenKhachSan,
                MoTa = khachSan.MoTa,
                DiaChi = khachSan.DiaChi,
                ChinhSachHuy = khachSan.ChinhSachHuy,
                SoSao = khachSan.SoSao,
                GioNhanPhong = khachSan.GioNhanPhong,
                GioTraPhong = khachSan.GioTraPhong,
                TenThanhPho = khachSan.Province?.full_name ?? string.Empty,
                TienIchs = khachSan.KhachSan_TienIches
                    .Where(kst => kst.TienIch != null)
                    .Select(kst => new AmenityDto
                    {
                        Id = kst.TienIch!.Id,
                        TenTienIch = kst.TienIch.TenTienIch,
                        Icon = kst.TienIch.Icon
                    }).ToList(),
                KhachSanImages = khachSan.KhachSanImages.Select(img => new HotelImageDto
                {
                    Id = img.Id,
                    Url = img.Url
                }).ToList()
            };
        }

        public async Task<bool> UpdateStatus(Guid hotelId, string status)
        {
            if (!Enum.TryParse<TrangThaiKhachSan>(status, true, out var parsedStatus))
            {
                return false;
            }

            return await _khachSanRepository.UpdateStatus(hotelId, parsedStatus.ToString());
        }

        public async Task<bool> UpdateHotel(Guid hotelId, Guid ownerId, UpdateHotelRequest request)
        {
            if (!TimeSpan.TryParse(request.GioNhanPhong, out var checkInTime) ||
                !TimeSpan.TryParse(request.GioTraPhong, out var checkOutTime) ||
                string.IsNullOrWhiteSpace(request.TenKhachSan) ||
                string.IsNullOrWhiteSpace(request.DiaChi) ||
                string.IsNullOrWhiteSpace(request.ThanhPho) ||
                request.SoSao < 1 || request.SoSao > 5 ||
                (request.ViDo.HasValue && (request.ViDo < -90 || request.ViDo > 90)) ||
                (request.KinhDo.HasValue && (request.KinhDo < -180 || request.KinhDo > 180)))
            {
                return false;
            }

            var hotel = await _khachSanRepository.GetHotelForOwner(hotelId, ownerId);
            if (hotel == null)
            {
                return false;
            }

            hotel.TenKhachSan = request.TenKhachSan.Trim();
            hotel.MoTa = request.MoTa?.Trim();
            hotel.ChinhSachHuy = request.ChinhSachHuy?.Trim();
            hotel.DiaChi = request.DiaChi.Trim();
            hotel.ThanhPho = request.ThanhPho.Trim();
            hotel.ViDo = request.ViDo;
            hotel.KinhDo = request.KinhDo;
            hotel.SoSao = request.SoSao;
            hotel.GioNhanPhong = checkInTime;
            hotel.GioTraPhong = checkOutTime;
            hotel.TrangThai = TrangThaiKhachSan.CHO_DUYET.ToString();

            return await _khachSanRepository.SaveHotelChanges(hotel);
        }

        public async Task<bool> AddHotelImages(Guid hotelId, Guid ownerId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0 || files.Count > 20 ||
                files.Any(file => file == null || file.Length == 0) ||
                await _khachSanRepository.GetHotelForOwner(hotelId, ownerId) == null)
            {
                return false;
            }

            var urls = new List<string>();
            foreach (var file in files)
            {
                urls.Add(await _cloudinaryService.UploadImageAsync(file));
            }

            await _khachSanImageRepository.InsertKhachSanImage(hotelId, urls);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteHotelImage(Guid hotelId, long imageId, Guid ownerId)
        {
            var image = await _khachSanImageRepository.GetImageForOwner(imageId, hotelId, ownerId);
            if (image == null || !await _khachSanImageRepository.DeleteImageForOwner(imageId, hotelId, ownerId))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(image.Url))
            {
                await _cloudinaryService.DeleteImageAsync(image.Url);
            }

            return true;
        }

        public async Task<HotelFilterResponse> FilterHotelsAsync(HotelFilterRequest dto)
        {
            var stopwatch = Stopwatch.StartNew();

                // 1. Tạo Cache Key chuẩn từ DTO
                string cacheKey = $"hotel-filter:" +
                                  $"{dto.Keyword ?? ""}:" +                            
                                  $"{dto.SoKhach?.ToString() ?? ""}:" +
                                  $"{dto.NgayNhanPhong?.ToString("yyyyMMdd") ?? ""}:" +
                                  $"{dto.NgayTraPhong?.ToString("yyyyMMdd") ?? ""}:" +
                                  $"{dto.Page}:" +
                                  $"{dto.PageSize}";

                // 2. Kiểm tra Cache trong Redis
                var cachedData = await _redisService.GetObject<HotelFilterResponse>(cacheKey);
                if (cachedData != null)
                {
                    stopwatch.Stop();
                    Booking.Common.Shared.FileLogger.Log($"[Service] Load từ Redis: {stopwatch.ElapsedMilliseconds} ms");
                    return cachedData;
                }

                // 3. Gọi Repository lấy danh sách Khách sạn và Tổng số lượng dòng (TotalRow)
                var (khachSans, totalRow) = await _khachSanRepository.FilterHotels(
                    dto.Keyword,              
                    dto.SoKhach,
                    dto.NgayNhanPhong,
                    dto.NgayTraPhong,
                    dto.Page,
                    dto.PageSize);

                // Nếu không tìm thấy khách sạn nào, trả về đối tượng rỗng
                if (khachSans == null || !khachSans.Any())
                {
                    return new HotelFilterResponse
                    {
                        Data = new List<HotelFilterItem>(),
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

                var priceLookup = prices
                    .Where(x => x?.LoaiPhong?.KhachSanId != null)
                    .GroupBy(x => x.LoaiPhong.KhachSanId!.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Min(x => x.Gia)
                    );
                // 5. Map dữ liệu từ Entity -> HotelFilterItem
                var hotelDtos = khachSans.Select(hotel => new HotelFilterItem
                {
                    Id = hotel.Id,
                    TenKhachSan = hotel.TenKhachSan,
                    MoTa = hotel.MoTa,
                    DiaChi = hotel.DiaChi,
                    SoSao = hotel.SoSao,
                    Gia = priceLookup.TryGetValue(hotel.Id, out var gia) ? gia : 0,
                    Urls = imageLookup.TryGetValue(hotel.Id, out var urls) ? urls : new List<string>()
                }).ToList();

                // 6. Tính tổng số trang bằng totalRow vừa nhận từ Repository
                int totalPage = Paginations.GetTotalPages(totalRow, dto.PageSize);

                var response = new HotelFilterResponse
                {
                    Data = hotelDtos,
                    TotalRow = totalRow,
                    TotalPage = totalPage
                };

                // 7. Lưu vào Redis (Cache trong 30 giây)
                await _redisService.SetObject(cacheKey, response, TimeSpan.FromSeconds(30));

                stopwatch.Stop();

                Booking.Common.Shared.FileLogger.Log($"[Service] FilterHotels DB Execution: {stopwatch.ElapsedMilliseconds} ms");

                return response;
        }



        // 1. Dành cho Admin: Lấy tất cả khách sạn theo bộ lọc
        public async Task<HotelFilterResponse> LayDanhSachKhachSanAdminAsync(AdminHotelFilterRequest dto)
        {
            var (khachSans, totalRow) = await _khachSanRepository.GetHotelsForAdmin(
                dto.Keyword,
                dto.ThanhPho,
                dto.ViDo ?? 0,
                dto.KinhDo ?? 0,
                dto.SoSao,
                dto.TrangThai,
                dto.Page,
                dto.PageSize
            );

            return await MapToHotelFilterResponseAsync(khachSans, totalRow, dto.PageSize);
        }

        // 2. Dành cho Owner: Lấy danh sách khách sạn do Owner đó sở hữu
        public async Task<HotelFilterResponse> LayDanhSachKhachSanOwnerAsync(OwnerHotelFilterRequest dto, Guid ownerId)
        {
            var (khachSans, totalRow) = await _khachSanRepository.GetHotelsForOwner(
                
                ownerId,
                dto.Page,
                dto.PageSize
            );

            return await MapToHotelFilterResponseAsync(khachSans, totalRow, dto.PageSize);
        }

        public async Task<List<OptionDto>> GetHotelOptionsByOwnerId(Guid ownerId)
        {
            var hotels = await _khachSanRepository.GetHotelOptionsByOwnerId(ownerId);

            return hotels.Select(hotel => new OptionDto
            {
                Id = hotel.Id,
                Name = hotel.TenKhachSan ?? string.Empty
            }).ToList();
        }

        // Private Helper: Xử lý gom nhóm Ảnh, Mapping DTO và Phân trang để dùng chung
        private async Task<HotelFilterResponse> MapToHotelFilterResponseAsync(
            List<KhachSan> khachSans,
            int totalRow,
            int pageSize)
        {
            // Nếu danh sách rỗng, trả về response trống lập tức
            if (khachSans == null || !khachSans.Any())
            {
                return new HotelFilterResponse
                {
                    Data = new List<HotelFilterItem>(),
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
            var hotelDtos = khachSans.Select(hotel => new HotelFilterItem
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

            return new HotelFilterResponse
            {
                Data = hotelDtos,
                TotalRow = totalRow,
                TotalPage = totalPage
            };
        }

        public async Task<CreateHotelResponse> CreateHotel(CreateHotelRequest ks, Guid nguoiTao)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (!TimeSpan.TryParse(ks.GioNhanPhong, out var gioNhan) ||
                    !TimeSpan.TryParse(ks.GioTraPhong, out var gioTra))
                {
                    return new CreateHotelResponse
                    {
                        Status = false,
                        Message = "Giờ nhận phòng hoặc giờ trả phòng không hợp lệ",
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
                    ChinhSachHuy = ks.ChinhSachHuy,
                    ThanhPho = ks.ThanhPho,
                    ViDo = ks.ViDo,
                    KinhDo = ks.KinhDo,
                    SoSao = ks.SoSao,
                    GioNhanPhong = Convert.ToDateTime(ks.GioNhanPhong).TimeOfDay,
                    GioTraPhong = Convert.ToDateTime(ks.GioTraPhong).TimeOfDay,
                    TrangThai = TrangThaiKhachSan.CHO_DUYET.ToString(),
                    NguoiTao = nguoiTao,
                    NgayTao = DateTime.Now
                };
                await _khachSanRepository.CreateHotel(KhachSan);
                await _khachSanImageRepository.InsertKhachSanImage(khachSanId, images);
                await _unitOfWork.CommitAsync();
                return new CreateHotelResponse
                {
                    Status = true,
                    Message = "Tạo khách sạn thành công",
                    Data = KhachSan
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
