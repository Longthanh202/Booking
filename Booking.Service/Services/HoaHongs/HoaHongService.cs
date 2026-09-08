using Booking.Common.Shared;
using Booking.Common.Shared.Enum.Booking;
using Booking.Common.Shared.Enum.Commission;
using Booking.Common.Shared.Enum.Payment;
using Booking.Common.Shared.Enum.Wallet;
using Booking.Core.Model.HoaHongs;
using Booking.Core.Model.LichSuVis;
using Booking.Core.Model.TienIchs;
using Booking.Data;
using Booking.Data.Repository.DatPhongs;
using Booking.Data.Repository.HoaHongs;
using Booking.Data.Repository.LichSuVis;
using Booking.Data.Repository.ThanhToans;
using Booking.Data.Repository.ViKhachSans;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Service.Dtos.HoaHong;

namespace Booking.Service.Services.HoaHongs
{
    internal class HoaHongService : IHoaHongService
    {
        private readonly IDatPhongRepository _datPhongRepository;
        private readonly IThanhToanRepository _thanhToanRepository;
        private readonly IHoaHongRepository _hoaHongRepository;
        private readonly IViKhachSanRepository _viRepository;
        private readonly ILichSuViRepository _lichSuViRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly decimal _tyLeHoaHong;

        public HoaHongService(
        IDatPhongRepository datPhongRepository,
        IThanhToanRepository thanhToanRepository,
        IHoaHongRepository hoaHongRepository,
        IViKhachSanRepository viRepository,
        ILichSuViRepository lichSuViRepository,
        IUnitOfWork unitOfWork,
        IConfiguration config)
        {
            _datPhongRepository = datPhongRepository;
            _thanhToanRepository = thanhToanRepository;
            _hoaHongRepository = hoaHongRepository;
            _viRepository = viRepository;
            _lichSuViRepository = lichSuViRepository;
            _unitOfWork = unitOfWork;
            _config = config;
            _tyLeHoaHong = config.GetValue<decimal>("HoaHong:TyLeHoaHong");
        }
        public async Task TinhHoaHong(Guid datPhongId)
        {         
            var datPhong = await _datPhongRepository.LayTheoId(datPhongId);

            if (datPhong == null)
                throw new Exception("Không tìm thấy đơn đặt phòng.");

            if (datPhong.TrangThai != TrangThaiDatPhong.DA_CHECK_OUT.ToString())
                throw new Exception("Đơn chưa checkout.");

            var daTinh = await _hoaHongRepository.LayTheoDatPhong(datPhongId);

            if (daTinh != null)
            {             
                return;
            }

            var thanhToan = await _thanhToanRepository.LayTheoDatPhong(datPhongId);

            if (thanhToan == null)
                throw new Exception("Không tìm thấy thanh toán.");


            if (!datPhong.TongTien.HasValue)
            {
                throw new Exception("Đơn đặt phòng chưa có tổng tiền.");
            }

            if (!datPhong.KhachSanId.HasValue)
                throw new Exception("Không xác định khách sạn.");

            if (_tyLeHoaHong <= 0 || _tyLeHoaHong > 100)
                throw new Exception("Tỷ lệ hoa hồng không hợp lệ.");


            decimal tongTien = datPhong.TongTien.Value;
            Guid khachSanId = datPhong.KhachSanId.Value;

            decimal tienHoaHong = tongTien * _tyLeHoaHong / 100;
            decimal tienChiTra = tongTien - tienHoaHong;


            await _unitOfWork.BeginTransactionAsync();

            try
            {              
                var hoaHong = new HoaHong
                {
                    MaDatPhong = datPhong.Id,
                    MaKhachSan = khachSanId,
                    TyLeHoaHong = _tyLeHoaHong,
                    SoTienHoaHong = tienHoaHong, 
                };            

                var vi = await _viRepository.LayTheoKhachSan(khachSanId);

                if (vi == null)
                    throw new Exception("Khách sạn chưa có ví.");
           

                if (thanhToan.PhuongThuc == PhuongThucThanhToan.TIEN_MAT.ToString())
                {
                    // Khách thanh toán trực tiếp cho khách sạn.
                    // Sàn chỉ ghi nhận khoản hoa hồng cần thu, chưa tác động đến ví.

                    hoaHong.TrangThai = TrangThaiHoaHong.CHO_THU.ToString();
                }
                else
                {
                    // Khách thanh toán qua sàn.
                    // Sàn giữ lại hoa hồng và cộng phần còn lại vào ví khách sạn.

                    hoaHong.TrangThai = TrangThaiHoaHong.DA_THU.ToString();

                    vi.SoDu += tienChiTra;

                    await _viRepository.CapNhat(vi);

                    await _lichSuViRepository.Tao(new LichSuVi
                    {
                        MaVi = vi.MaVi,
                        MaDatPhong = datPhong.Id,
                        SoTien = tienChiTra,
                        LoaiGiaoDich = LoaiGiaoDichVi.CHI_TRA_BOOKING.ToString(),
                        NgayTao = DateTime.Now
                    });
                }
                await _hoaHongRepository.Tao(hoaHong);
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<List<HoaHongDto>> LayTheoOwnerId(Guid ownerId)
        {
            var result = await _hoaHongRepository.LayTheoOwnerId(ownerId);

            return result.Select(x => new HoaHongDto
            {
                MaHoaHong = x.MaHoaHong,
                MaDatPhong = x.MaDatPhong,
                MaKhachSan = x.MaKhachSan,
                TenKhachSan = x.KhachSan?.TenKhachSan ?? "",
                TyLeHoaHong = x.TyLeHoaHong,
                SoTienHoaHong = x.SoTienHoaHong,
                TrangThai = x.TrangThai,
                NgayTao = x.NgayTao,
                NgayThu = x.NgayThu
            }).ToList();
        }
    }
}
