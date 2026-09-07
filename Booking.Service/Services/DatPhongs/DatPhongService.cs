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
using Booking.Service.Dtos.DatPhongs;
using Booking.Service.Dtos.Email;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Service.Services.Notifications;

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

        public DatPhongService(IDatPhongRepository datPhongRepository,
            IThanhToanRepository thanhToanRepository,
            IUnitOfWork unitOfWork,
            IGiaPhongRepository giaPhongRepository,
            IPhongRepository phongRepository, 
            IRabbitMQPublisher rabbitMQPublisher,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _datPhongRepository = datPhongRepository;
            _thanhToanRepository = thanhToanRepository;
            _unitOfWork = unitOfWork;
            _giaPhongRepository = giaPhongRepository;
            _phongRepository = phongRepository;
            _rabbitMQPublisher = rabbitMQPublisher;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<DatPhong> CapNhatTrangThai(Guid id)
        {
            var datPhong = await _datPhongRepository.LayTheoId(id);

            if (datPhong == null)
                throw new Exception("Không tìm thấy đơn đặt phòng.");

            if (datPhong.TrangThai != TrangThaiDatPhong.DA_CHECK_IN.ToString())
                throw new Exception("Chỉ có thể checkout khi đang check-in.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                datPhong.TrangThai = TrangThaiDatPhong.DA_CHECK_OUT.ToString();

                await _datPhongRepository.CapNhatTrangThai(datPhong);

                await _unitOfWork.CommitAsync();

                var check = await _datPhongRepository.LayTheoId(id);
                        
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

        public async Task<DatPhongOwnerDto> GetListBookingOwner(
            DatPhongOwnerRequest dto,
            Guid ownerId)
            {
            var page = dto.Page <= 0 ? 1 : dto.Page;
            var pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

            var result = await _datPhongRepository.GetListBookingOwner(
                ownerId,
                dto.khachSanId,
                page,
                pageSize);

            var data = result.Items.Select(x => new DatPhongDto
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
                    .Select(ct => new ChiTietDatPhongDto
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
                            .Select(pd => new PhongDto
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

            return new DatPhongOwnerDto
            {
                Data = data,
                TotalRow = result.TotalCount,
                TotalPage = totalPage
            };
        }

        public async Task<BookingHistory> BookingHistory(Guid userId, int pageIndex, int pageSize)
        {
            var page = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var result = await _datPhongRepository.BookingHistory(
                userId,
                page,
                pageSize);

            var data = result.Items.Select(x => new DatPhongDto
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
                    .Select(ct => new ChiTietDatPhongDto
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
                            .Select(pd => new PhongDto
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

            return new BookingHistory
            {
                Data = data,
                TotalRow = result.TotalCount,
                TotalPage = totalPage
            };
        }

        public async Task CheckIn(Guid id)
        {
            var datPhong = await _datPhongRepository.LayTheoId(id);

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

            if (datPhong.TrangThai == TrangThaiDatPhong.DA_CHECK_IN.ToString())
            {
                throw new Exception("Booking đã check-in");
            }

            if (datPhong.TrangThai == TrangThaiDatPhong.DA_CHECK_OUT.ToString())
            {
                throw new Exception("Booking đã check-out");
            }

            await _datPhongRepository.CheckIn(id);
        }

        public async Task XacNhan(Guid id)
        {
            var datPhong = await _datPhongRepository.LayTheoId(id);

            if (datPhong == null)
            {
                throw new Exception("Booking không tồn tại");
            }

            if (datPhong.TrangThai == TrangThaiDatPhong.DA_CHECK_IN.ToString())
            {
                throw new Exception("Booking đã check-in");
            }

            if (datPhong.TrangThai == TrangThaiDatPhong.DA_CHECK_OUT.ToString())
            {
                throw new Exception("Booking đã check-out");
            }

            await _datPhongRepository.XacNhan(id);
        }

        public async Task<DatPhong> DatPhong(DatPhongRequest dp, Guid userId)
        {
            if (!dp.NgayNhanPhong.HasValue || !dp.NgayTraPhong.HasValue)
            {
                throw new Exception("Ngày nhận phòng và ngày trả phòng không được để trống.");
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

                var datPhong = new DatPhong
                {
                    Id = datPhongId,
                    KhachHangId = userId,
                    KhachSanId = dp.KhachSanId,
                    NgayNhanPhong = dp.NgayNhanPhong,
                    NgayTraPhong = dp.NgayTraPhong,
                    TongTien = tongTien,
                    TrangThai = TrangThaiDatPhong.CHO_XAC_NHAN.ToString(),
                    NgayTao = DateTime.Now
                };

                // 7. Lưu tất cả thông tin vào DB
                await _datPhongRepository.DatPhong(datPhong, ctdps, phongDats);

                await _thanhToanRepository.Tao(new ThanhToan
                {
                    DatPhongId = datPhongId,
                    PhuongThuc = dp.PhuongThucThanhToan,
                    SoTien = tongTien,
                    TrangThai = TrangThaiThanhToan.CHO_THANH_TOAN.ToString()
                });

                await _unitOfWork.CommitAsync();
                await _notificationService.BookingSuccess(userId, datPhong);

                var user = await _userRepository.GetById(userId);

                await _rabbitMQPublisher.PublishAsync(
                    "booking_email_queue",
                    new DatPhongEvent
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
    }
}
