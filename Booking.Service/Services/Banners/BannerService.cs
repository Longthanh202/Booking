
using Booking.Core.Model.Banners;
using Booking.Data.Repository.Banners;
using Booking.Service.Dtos.Banner;
using Booking.Service.Services.Cloudinarys;


namespace Booking.Service.Services.Banners
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly CloudinaryService _cloudinaryService;
        public BannerService(IBannerRepository bannerRepository, CloudinaryService cloudinaryService)
        {
            _bannerRepository = bannerRepository;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<List<Banner>> GetAllBanner(string keyword, int isActive, int startRow, int endRow)
        {
            return await _bannerRepository.GetAllBanner(keyword, isActive, startRow, endRow);
        }

        public async Task<List<Banner>> GetBannerIsActive()
        {
            return await _bannerRepository.GetBannerIsActive();
        }

        public async Task<Banner> InsertBanner(InsertBannerDto banner)
        {
            var url = await _cloudinaryService.UploadImageAsync(banner.File);
            var input = new Banner
            {
                Title = banner.Title,
                Subtitle = banner.Subtitle,
                IsActive = banner.IsActive,
                Url = url
            };
            return await _bannerRepository.InsertBanner(input);
        }
    }
}
