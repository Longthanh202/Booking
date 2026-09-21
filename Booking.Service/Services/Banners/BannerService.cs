
using Booking.Core.Model.Banners;
using Booking.Data.Repository.Banners;
using Booking.Service.Dtos.Banners;
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
        public async Task<List<Banner>> GetBanners(string keyword, int isActive, int startRow, int endRow)
        {
            return await _bannerRepository.GetBanners(keyword, isActive, startRow, endRow);
        }

        public async Task<List<Banner>> GetActiveBanners()
        {
            return await _bannerRepository.GetActiveBanners();
        }

        public async Task<Banner> CreateBanner(CreateBannerRequest banner)
        {
            var url = await _cloudinaryService.UploadImageAsync(banner.File);
            var input = new Banner
            {
                Title = banner.Title,
                Subtitle = banner.Subtitle,
                IsActive = banner.IsActive,
                Url = url
            };
            return await _bannerRepository.CreateBanner(input);
        }
    }
}
