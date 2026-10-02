using Booking.Core.Model.Banners;
using Booking.Service.Dtos.Banners;

namespace Booking.Data.Repository.Banners
{
    public interface IBannerService
    {
        Task<Banner> CreateBanner(CreateBannerRequest banner);
        Task<List<Banner>> GetBanners(string keyword, int isActive, int startRow, int endRow);

        Task<List<Banner>> GetActiveBanners();
        Task<Banner?> UpdateBanner(long id, UpdateBannerRequest request);
        Task<bool> DeleteBanner(long id);
    }
}
