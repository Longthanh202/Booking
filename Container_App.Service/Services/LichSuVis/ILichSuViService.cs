using Container_App.Service.Dtos.LichSuVi;

namespace Container_App.Service.Services.LichSuVis;

public interface ILichSuViService
{
    Task<List<LichSuViDto>> LayLichSuViOwner(Guid ownerId);
}