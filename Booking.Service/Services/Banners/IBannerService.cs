using Booking.Core.Model.Banners;
using Booking.Service.Dtos.Banner;

namespace Booking.Data.Repository.Banners
{
    public interface IBannerService
    {
        Task<Banner> CreateBanner(InsertBannerDto banner);
        Task<List<Banner>> GetBanners(string keyword, int isActive, int startRow, int endRow);

        Task<List<Banner>> GetActiveBanners();
    }
}
