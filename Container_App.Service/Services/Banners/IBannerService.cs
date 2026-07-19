using Container_App.Core.Model.Banners;
using Container_App.Service.Dtos.Banner;

namespace Container_App.Data.Repository.Banners
{
    public interface IBannerService
    {
        Task<Banner> InsertBanner(InsertBannerDto banner);
        Task<List<Banner>> GetAllBanner(string keyword, int isActive, int startRow, int endRow);

        Task<List<Banner>> GetBannerIsActive();
    }
}
