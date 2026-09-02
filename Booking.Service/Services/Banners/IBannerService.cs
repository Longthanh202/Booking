using Booking.Core.Model.Banners;
using Booking.Service.Dtos.Banner;

namespace Booking.Data.Repository.Banners
{
    public interface IBannerService
    {
        Task<Banner> InsertBanner(InsertBannerDto banner);
        Task<List<Banner>> GetAllBanner(string keyword, int isActive, int startRow, int endRow);

        Task<List<Banner>> GetBannerIsActive();
    }
}
