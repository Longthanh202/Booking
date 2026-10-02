using Booking.Common.Shared;
using Booking.Common.Shared.Enum.Booking;
using Booking.Common.Shared.Enum.Payment;
using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.PhongDats;
using Booking.Data;
using Booking.Data.Connection;
using Booking.Data.Repository.DatPhongs;
using Booking.Data.Repository.GiaPhongs;
using Booking.Data.Repository.KhachSans;
using Booking.Data.Repository.PhongDats;
using Booking.Data.Repository.Phongs;
using Booking.Data.Repository.RabbitMQ;
using Booking.Data.Repository.ThanhToans;
using Booking.Data.Repository.Users;
using Booking.Service.Dtos.Bookings;
using Booking.Service.Dtos.Common;
using Booking.Service.Dtos.Email;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Service.Services.Notifications;
using Booking.Service.Services.Promotions;

namespace Booking.Service.Services.DatPhongs
{
    public class DatPhongService : IDatPhongService
    {
        private readonly IDatPhongRepository _datPhongRepository;
        private readonly IThanhToanRepository _thanhToanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGiaPhongRepository _giaPhongRepository;
        private readonly IPhongRepository _phongRepository;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IHotelPromotionService _hotelPromotionService;

        public DatPhongService(IDatPhongRepository datPhongRepository,
            IThanhToanRepository thanhToanRepository,
            IUnitOfWork unitOfWork,
            IGiaPhongRepository giaPhongRepository,
            IPhongRepository phongRepository, 
            IRabbitMQPublisher rabbitMQPublisher,
            IUserRepository userRepository,
            INotificationService notificationService,
            IHotelPromotionService hotelPromotionService)
        {
            _datPhongRepository = datPhongRepository;
            _thanhToanRepository = thanhToanRepository;
            _unitOfWork = unitOfWork;
            _giaPhongRepository = giaPhongRepository;
            _phongRepository = phongRepository;
            _rabbitMQPublisher = rabbitMQPublisher;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _hotelPromotionService = hotelPromotionService;
        }

        public async Task<DatPhong> UpdateBookingStatus(Guid id, Guid ownerId)
        {
            if (!await _datPhongRepository.IsBookingOwnedBy(id, ownerId))
                throw new UnauthorizedAccessException("Booking không thuộc property của owner.");

            var datPhong = await _datPhongRepository.GetBookingById(id);

            if (datPhong == null)
                throw new Exception("Không tìm thấy đơn đặt phòng.");

            if (datPhong.TrangThai != TrangThaiDatPhong.DA_CHECK_IN.ToString())
                throw new Exception("Chỉ có thể checkout khi đang check-in.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                datPhong.TrangThai = TrangThaiDatPhong.DA_CHECK_OUT.ToString();

                await _datPhongRepository.UpdateBookingStatus(datPhong);

                await _unitOfWork.CommitAsync();

                if (datPhong.KhachHangId.HasValue)
                {
                    await _notificationService.BookingStatusChanged(
                        datPhong.KhachHangId.Value, datPhong, "Booking đã check-out.");
                }

                var check = await _datPhongRepository.GetBookingById(id);
                        
                await _rabbitMQPublisher.PublishAsync(
                    "booking_checked_out",
                    new BookingCheckedOutEvent
                    {
                        DatPhongId = datPhong.Id
                    });

                return datPhong;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<OwnerBookingResponse> GetOwnerBookings(
            OwnerBookingSearchRequest dto,
            Guid ownerId)
            {
            var page = dto.Page <= 0 ? 1 : dto.Page;
            var pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

            var result = await _datPhongRepository.GetOwnerBookings(
                ownerId,
                dto.KhachSanId,
                page,
                pageSize);

            var data = result.Items.Select(x => new BookingDto
            {
                // =========================
                // Đặt phòng
                // =========================
                Id = x.Id,
                TrangThai = x.TrangThai,
                NgayTao = x.NgayTao,
                NgayNhan = x.NgayNhanPhong,
                NgayTra = x.NgayTraPhong,

                // =========================
                // Khách sạn
                // =========================
                KhachSanId = x.KhachSanId,
                TenKhachSan = x.KhachSan?.TenKhachSan,

                // =========================
                // Thanh toán
                // =========================
                ThanhToan = x.ThanhToans
                    .Select(t => t.PhuongThuc)
                    .FirstOrDefault(),

                // =========================
                // Chi tiết đặt phòng
                // =========================
                ChiTietDatPhongs = x.ChiTietDatPhongs
                    .Select(ct => new BookingRoomTypeDetailDto
                    {
                        Id = ct.Id,

                        // Loại phòng
                        LoaiPhongId = ct.LoaiPhongId,

                        TenLoaiPhong = ct.LoaiPhong?.TenLoaiPhong,

                        Gia = ct.GiaMoiDem,

                        // =========================
                        // Các phòng được đặt
                        // =========================
                        Phongs = ct.PhongDats
                            .Select(pd => new BookedRoomDto
                            {
                                Id = pd.Phong.Id,
                                SoPhong = pd.Phong.SoPhong,
                                TrangThai = pd.Phong.TrangThai
                            })
                            .ToList()
                    })
                    .ToList(),
                TongTien = x.TongTien,
                SoTienGiam = x.SoTienGiam,
                MaKhuyenMai = x.MaKhuyenMai

            }).ToList();

            var totalPage = (int)Math.Ceiling(
                (double)result.TotalCount / pageSize);

            return new OwnerBookingResponse
            {
                Data = data,
                TotalRow = result.TotalCount,
                TotalPage = totalPage
            };
        }

        public async Task<BookingHistoryResponse> GetBookingHistory(Guid userId, int pageIndex, int pageSize)
        {
            var page = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var result = await _datPhongRepository.GetBookingHistory(
                userId,
                page,
                pageSize);

            var data = result.Items.Select(x => new BookingDto
            {
                // =========================
                // Đặt phòng
                // =========================
                Id = x.Id,
                TrangThai = x.TrangThai,
                NgayTao = x.NgayTao,

                // =========================
                // Khách sạn
                // =========================
                KhachSanId = x.KhachSanId,
                TenKhachSan = x.KhachSan?.TenKhachSan,

                // =========================
                // Thanh toán
                // =========================
                ThanhToan = x.ThanhToans
                    .Select(t => t.PhuongThuc)
                    .FirstOrDefault(),

                // =========================
                // Chi tiết đặt phòng
                // =========================
                ChiTietDatPhongs = x.ChiTietDatPhongs
                    .Select(ct => new BookingRoomTypeDetailDto
                    {
                        Id = ct.Id,

                        // Loại phòng
                        LoaiPhongId = ct.LoaiPhongId,

                        TenLoaiPhong = ct.LoaiPhong?.TenLoaiPhong,

                        Gia = ct.GiaMoiDem,

                        // =========================
                        // Các phòng được đặt
                        // =========================
                        Phongs = ct.PhongDats
                            .Select(pd => new BookedRoomDto
                            {
                                Id = pd.Phong.Id,
                                SoPhong = pd.Phong.SoPhong,
                                TrangThai = pd.Phong.TrangThai
                            })
                            .ToList()
                    })
                    .ToList(),
                TongTien = x.TongTien,
                SoTienGiam = x.SoTienGiam,
                MaKhuyenMai = x.MaKhuyenMai

            }).ToList();

            var totalPage = (int)Math.Ceiling(
                (double)result.TotalCount / pageSize);

            return new BookingHistoryResponse
            {
                Data = data,
                TotalRow = result.TotalCount,
                TotalPage = totalPage
            };
        }

        public async Task<List<OptionDto>> GetCustomerOptionsByHotelId(Guid hotelId, Guid ownerId)
        {
            var customers = await _datPhongRepository.GetCustomerOptionsByHotelId(hotelId, ownerId);

            return customers.Select(customer => new OptionDto
            {
                Id = customer.Id,
                Name = customer.FullName ?? string.Empty
            }).ToList();
        }

        public Task<DatPhong?> GetCustomerBookingById(Guid id, Guid customerId) =>
            _datPhongRepository.GetCustomerBookingById(id, customerId);

        public async Task<bool> CancelBooking(Guid id, Guid customerId)
        {
            var changed = await _datPhongRepository.CancelBooking(id, customerId);
            var booking = changed ? await _datPhongRepository.GetBookingById(id) : null;
            if (booking?.KhachSan != null)
            {
                await _notificationService.BookingStatusChanged(
                    booking.KhachSan.NguoiTao, booking, "Khách đã hủy booking.");
            }
            return changed;
        }

        public async Task<bool> RejectBooking(Guid id, Guid ownerId)
        {
            var changed = await _datPhongRepository.ChangeOwnerBookingStatus(
                id, ownerId, TrangThaiDatPhong.TU_CHOI.ToString());
            var booking = changed ? await _datPhongRepository.GetBookingById(id) : null;
            if (booking?.KhachHangId.HasValue == true)
            {
                await _notificationService.BookingStatusChanged(
                    booking.KhachHangId.Value, booking, "Booking đã bị từ chối.");
            }
            return changed;
        }

        public async Task<bool> CancelBookingByOwner(Guid id, Guid ownerId)
        {
            var changed = await _datPhongRepository.ChangeOwnerBookingStatus(
                id, ownerId, TrangThaiDatPhong.DA_HUY.ToString());
            var booking = changed ? await _datPhongRepository.GetBookingById(id) : null;
            if (booking?.KhachHangId.HasValue == true)
            {
                await _notificationService.BookingStatusChanged(
                    booking.KhachHangId.Value, booking, "Booking đã bị hủy bởi khách sạn.");
            }
            return changed;
        }

        public async Task CheckIn(Guid id, Guid ownerId)
        {
            if (!await _datPhongRepository.IsBookingOwnedBy(id, ownerId))
                throw new UnauthorizedAccessException("Booking không thuộc property của owner.");

            var datPhong = await _datPhongRepository.GetBookingById(id);

            if (datPhong == null)
            {
                throw new Exception("Booking không tồn tại");
            }
            if (!datPhong.NgayNhanPhong.HasValue)
            {
                throw new Exception("Booking chưa có ngày nhận phòng");
            }
            var today = DateTime.Now.Date;
            var ngayNhanPhong = datPhong.NgayNhanPhong.Value.Date;
            if (today < ngayNhanPhong)
            {
                throw new Exception("Chưa đến ngày check-in");
            }

            if (datPhong.TrangThai != TrangThaiDatPhong.DA_XAC_NHAN.ToString())
            {
                throw new Exception("Chỉ có thể check-in booking đã xác nhận.");
            }

            var checkedInBooking = await _datPhongRepository.CheckIn(id);
            if (checkedInBooking.KhachHangId.HasValue)
            {
                await _notificationService.BookingStatusChanged(
                    checkedInBooking.KhachHangId.Value, checkedInBooking, "Booking đã check-in.");
            }
        }

        public async Task ConfirmBooking(Guid id, Guid ownerId)
        {
            if (!await _datPhongRepository.IsBookingOwnedBy(id, ownerId))
                throw new UnauthorizedAccessException("Booking không thuộc property của owner.");

            var datPhong = await _datPhongRepository.GetBookingById(id);

            if (datPhong == null)
            {
                throw new Exception("Booking không tồn tại");
            }

            if (datPhong.TrangThai != TrangThaiDatPhong.CHO_XAC_NHAN.ToString())
            {
                throw new Exception("Chỉ có thể xác nhận booking đang chờ xác nhận.");
            }

            var confirmedBooking = await _datPhongRepository.ConfirmBooking(id);
            if (confirmedBooking.KhachHangId.HasValue)
            {
                await _notificationService.BookingStatusChanged(
                    confirmedBooking.KhachHangId.Value, confirmedBooking, "Booking đã được xác nhận.");
            }
        }

        public async Task<DatPhong> CreateBooking(CreateBookingRequest dp, Guid userId)
        {
            if (!dp.NgayNhanPhong.HasValue || !dp.NgayTraPhong.HasValue ||
                dp.NgayNhanPhong >= dp.NgayTraPhong || dp.DanhSachPhong == null || dp.DanhSachPhong.Count == 0)
            {
                throw new ArgumentException("Ngày nhận/trả phòng và danh sách phòng phải hợp lệ.");
            }

            // Dùng .Value để ép về kiểu TimeSpan không null
            int soNgayO = (dp.NgayTraPhong.Value - dp.NgayNhanPhong.Value).Days;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                decimal tongTien = 0;
                var ctdps = new List<ChiTietDatPhong>();
                var phongDats = new List<PhongDat>(); // Bảng lưu CTDP_Id và PhongId
                var datPhongId = Guid.NewGuid();

                foreach (var item in dp.DanhSachPhong)
                {
                    if (item.SoLuong <= 0 ||
                        !await _datPhongRepository.IsRoomTypeForHotel(item.LoaiPhongId, dp.KhachSanId))
                    {
                        throw new ArgumentException("Loại phòng không thuộc khách sạn hoặc số lượng phòng không hợp lệ.");
                    }

                    // 2. Kiểm tra & lấy danh sách phòng còn trống trong khoảng thời gian dp.NgayNhanPhong -> dp.NgayTraPhong
                    var phongTrongs = await _phongRepository.LayDanhSachPhongTrong(
                        item.LoaiPhongId,
                        dp.NgayNhanPhong,
                        dp.NgayTraPhong
                    );

                    if (phongTrongs.Count < item.SoLuong)
                    {
                        throw new Exception($"Loại phòng ID {item.LoaiPhongId} không đủ phòng trống trong thời gian này.");
                    }

                    // 3. Tự động lấy số lượng phòng tương ứng mà khách đặt
                    var phongDuocChon = phongTrongs.Take(item.SoLuong).ToList();

                    // 4. Lấy giá phòng và tính tiền
                    var giaPhong = await _giaPhongRepository.LayGiaPhongHienTai(item.LoaiPhongId);
                    if (giaPhong == null)
                    {
                        throw new Exception($"Không tìm thấy bảng giá áp dụng cho loại phòng ID: {item.LoaiPhongId}");
                    }
                    tongTien += (giaPhong.Gia * item.SoLuong * soNgayO);

                    // 5. Tạo ChiTietDatPhong
                    var ctdp = new ChiTietDatPhong
                    {
                        Id = Guid.NewGuid(), // Gán GUID chủ động để liên kết với PhongDat
                        DatPhongId = datPhongId,
                        LoaiPhongId = item.LoaiPhongId,
                        SoLuongPhong = item.SoLuong,
                        GiaMoiDem = giaPhong.Gia,
                    };
                    ctdps.Add(ctdp);

                    // 6. Gán các phòng cụ thể vào bảng PhongDat
                    foreach (var phong in phongDuocChon)
                    {
                        phongDats.Add(new PhongDat
                        {
                            Id = Guid.NewGuid(),
                            ChiTietDatPhongId = ctdp.Id,
                            PhongId = phong.Id
                        });
                    }
                }

                decimal soTienGiam = 0;
                string? maKhuyenMai = null;
                if (!string.IsNullOrWhiteSpace(dp.MaKhuyenMai))
                {
                    var promotion = await _hotelPromotionService.ApplyForBooking(
                        dp.KhachSanId, dp.MaKhuyenMai, tongTien);
                    if (promotion == null)
                    {
                        throw new PromotionNotApplicableException();
                    }

                    soTienGiam = promotion.DiscountAmount;
                    maKhuyenMai = promotion.Code;
                    tongTien -= soTienGiam;
                }

                var datPhong = new DatPhong
                {
                    Id = datPhongId,
                    KhachHangId = userId,
                    KhachSanId = dp.KhachSanId,
                    NgayNhanPhong = dp.NgayNhanPhong,
                    NgayTraPhong = dp.NgayTraPhong,
                    TongTien = tongTien,
                    SoTienGiam = soTienGiam,
                    MaKhuyenMai = maKhuyenMai,
                    TrangThai = TrangThaiDatPhong.CHO_XAC_NHAN.ToString(),
                    NgayTao = DateTime.Now
                };

                // 7. Lưu tất cả thông tin vào DB
                await _datPhongRepository.CreateBooking(datPhong, ctdps, phongDats);

                    await _thanhToanRepository.CreatePayment(new ThanhToan
                {
                    DatPhongId = datPhongId,
                    PhuongThuc = dp.PhuongThucThanhToan,
                    SoTien = tongTien,
                    TrangThai = TrangThaiThanhToan.CHO_THANH_TOAN.ToString()
                });

                await _unitOfWork.CommitAsync();
                await _notificationService.BookingSuccess(userId, datPhong);
                var createdBooking = await _datPhongRepository.GetBookingById(datPhong.Id);
                if (createdBooking?.KhachSan != null)
                {
                    await _notificationService.BookingCreatedForOwner(
                        createdBooking.KhachSan.NguoiTao, createdBooking);
                }

                var user = await _userRepository.GetUserProfileById(userId);

                await _rabbitMQPublisher.PublishAsync(
                    "booking_email_queue",
                    new BookingCreatedEvent
                {
                    BookingId = datPhong.Id,
                    HoTen = user.FullName,
                    Email = user.Email,
                    TenKhachSan = "Tên Khách Sạn",
                    NgayNhanPhong = dp.NgayNhanPhong.Value,
                    NgayTraPhong = dp.NgayTraPhong.Value,
                    TongTien = tongTien
                });

                return datPhong;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<(int datPhong, double tongTien)> GetOwnerBookingStatistics(
            OwnerBookingStatisticsRequest input)
        {
            try
            {
                // Validate input
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input), "Dữ liệu thống kê không được null");
                }

                if (input.HotelId == Guid.Empty)
                {
                    throw new ArgumentException(
                        "HotelId không hợp lệ",
                        nameof(input.HotelId));
                }

                if (input.StartDate == default)
                {
                    throw new ArgumentException(
                        "Ngày bắt đầu không hợp lệ",
                        nameof(input.StartDate));
                }

                if (input.EndDate == default)
                {
                    throw new ArgumentException(
                        "Ngày kết thúc không hợp lệ",
                        nameof(input.EndDate));
                }

                if (input.StartDate > input.EndDate)
                {
                    throw new ArgumentException(
                        "Ngày bắt đầu không được lớn hơn ngày kết thúc");
                }

                if ((input.EndDate - input.StartDate).TotalDays > 365)
                {
                    throw new ArgumentException(
                        "Khoảng thời gian thống kê không được vượt quá 365 ngày");
                }

                if (!string.IsNullOrWhiteSpace(input.Status))
                {
                    input.Status = input.Status.Trim();
                }

                return await _datPhongRepository.GetOwnerBookingStatistics(
                    input.HotelId,
                    input.StartDate,
                    input.EndDate,
                    input.Status
                );
            }
            catch (Exception ex)
            {
                FileLogger.Log(ex);
                throw;
            }
        }

        public async Task<OwnerBookingResponse> GetBookingsByOwner(OwnerBookingListRequest dto, Guid ownerId)
        {
            var page = dto.Page <= 0 ? 1 : dto.Page;
            var pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

            var result = await _datPhongRepository.GetBookingsByOwner(
                ownerId,
                dto.hotelId,
                dto.customerId,
                dto.trangThai,
                dto.ngayTao,
                page,
                pageSize);

            var data = result.Items.Select(x => new BookingDto
            {
                // =========================
                // Đặt phòng
                // =========================
                Id = x.Id,
                TrangThai = x.TrangThai,
                NgayTao = x.NgayTao,
                NgayNhan = x.NgayNhanPhong,
                NgayTra = x.NgayTraPhong,

                // =========================
                // Khách sạn
                // =========================
                KhachSanId = x.KhachSanId,
                TenKhachSan = x.KhachSan?.TenKhachSan,

                // =========================
                // Thanh toán
                // =========================
                ThanhToan = x.ThanhToans
                    .Select(t => t.PhuongThuc)
                    .FirstOrDefault(),

                // =========================
                // Chi tiết đặt phòng
                // =========================
                ChiTietDatPhongs = x.ChiTietDatPhongs
                    .Select(ct => new BookingRoomTypeDetailDto
                    {
                        Id = ct.Id,

                        // Loại phòng
                        LoaiPhongId = ct.LoaiPhongId,

                        TenLoaiPhong = ct.LoaiPhong?.TenLoaiPhong,

                        Gia = ct.GiaMoiDem,

                        // =========================
                        // Các phòng được đặt
                        // =========================
                        Phongs = ct.PhongDats
                            .Select(pd => new BookedRoomDto
                            {
                                Id = pd.Phong.Id,
                                SoPhong = pd.Phong.SoPhong,
                                TrangThai = pd.Phong.TrangThai
                            })
                            .ToList()
                    })
                    .ToList()

            }).ToList();

            var totalPage = (int)Math.Ceiling(
                (double)result.TotalCount / pageSize);

            return new OwnerBookingResponse
            {
                Data = data,
                TotalRow = result.TotalCount,
                TotalPage = totalPage
            };
        }

        public Task<OwnerBookingResponse> GetAdminBookings(OwnerBookingListRequest dto) =>
            GetBookingsByOwner(dto, Guid.Empty);
    }
}
