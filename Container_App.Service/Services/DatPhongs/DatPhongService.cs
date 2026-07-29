using Container_App.Common.Shared;
using Container_App.Common.Shared.Enum.Booking;
using Container_App.Common.Shared.Enum.Payment;
using Container_App.Core.Model.DatPhongs;
using Container_App.Core.Model.PhongDats;
using Container_App.Data;
using Container_App.Data.Connection;
using Container_App.Data.Repository.DatPhongs;
using Container_App.Data.Repository.GiaPhongs;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.PhongDats;
using Container_App.Data.Repository.Phongs;
using Container_App.Data.Repository.RabbitMQ;
using Container_App.Data.Repository.ThanhToans;
using Container_App.Data.Repository.Users;
using Container_App.Service.Dtos.DatPhongs;
using Container_App.Service.Dtos.Email;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.DatPhongs
{
    public class DatPhongService : IDatPhongService
    {
        private readonly IDatPhongRepository _datPhongRepository;
        private readonly IPhongDatRepository _phongDatRepository;
        private readonly IThanhToanRepository _thanhToanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGiaPhongRepository _giaPhongRepository;
        private readonly IPhongRepository _phongRepository;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        private readonly IUserRepository _userRepository;
        private readonly IKhachSanRepository _khachSanRepository;

        public DatPhongService(IDatPhongRepository datPhongRepository,
            IPhongDatRepository phongDatRepository,
            IThanhToanRepository thanhToanRepository,
            IUnitOfWork unitOfWork,
            IGiaPhongRepository giaPhongRepository,
            IPhongRepository phongRepository, 
            IRabbitMQPublisher rabbitMQPublisher,
            IUserRepository userRepository,
            IKhachSanRepository khachSanRepository)
        {
            _datPhongRepository = datPhongRepository;
            _phongDatRepository = phongDatRepository;
            _thanhToanRepository = thanhToanRepository;
            _unitOfWork = unitOfWork;
            _giaPhongRepository = giaPhongRepository;
            _phongRepository = phongRepository;
            _rabbitMQPublisher = rabbitMQPublisher;
            _userRepository = userRepository;
            _khachSanRepository = khachSanRepository;
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
                    TrangThai = TrangThaiDatPhong.DA_XAC_NHAN.ToString()
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
