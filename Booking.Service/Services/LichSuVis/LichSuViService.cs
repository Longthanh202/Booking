using Booking.Data.Repository.LichSuVis;
using Booking.Service.Dtos.HoaHong;
using Booking.Service.Dtos.LichSuVi;

namespace Booking.Service.Services.LichSuVis
{
    public class LichSuViService : ILichSuViService
    {
        private readonly ILichSuViRepository _lichSuViRepository;

        public LichSuViService(ILichSuViRepository lichSuViRepository)
        {
            _lichSuViRepository = lichSuViRepository;
        }
        public async Task<List<LichSuViDto>> LayLichSuViOwner(Guid ownerId)
        {
            var result = await _lichSuViRepository.LayTheoOwnerId(ownerId);

            return result.Select(x => new LichSuViDto
            {
                MaLichSu = x.MaLichSu,
                MaDatPhong = x.MaDatPhong,
                SoTien = x.SoTien,
                LoaiGiaoDich = x.LoaiGiaoDich,
                NoiDung = x.NoiDung,
                NgayTao = x.NgayTao
            }).ToList();
        }
    }
}

