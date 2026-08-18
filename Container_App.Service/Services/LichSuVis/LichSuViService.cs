using Container_App.Data.Repository.LichSuVis;
using Container_App.Service.Dtos.HoaHong;
using Container_App.Service.Dtos.LichSuVi;

namespace Container_App.Service.Services.LichSuVis;

public class LichSuViService: ILichSuViService
{
    private readonly ILichSuViRepository  _lichSuViRepository;

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