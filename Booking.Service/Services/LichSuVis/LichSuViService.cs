using Booking.Data.Repository.LichSuVis;
using Booking.Service.Dtos.Wallet;

namespace Booking.Service.Services.LichSuVis
{
    public class LichSuViService : ILichSuViService
    {
        private readonly ILichSuViRepository _lichSuViRepository;

        public LichSuViService(ILichSuViRepository lichSuViRepository)
        {
            _lichSuViRepository = lichSuViRepository;
        }
        public async Task<List<WalletHistoryDto>> GetOwnerWalletHistory(Guid ownerId)
        {
            var result = await _lichSuViRepository.GetWalletHistoryByOwnerId(ownerId);

            return result.Select(x => new WalletHistoryDto
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

