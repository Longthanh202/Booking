
using Container_App.Core.Model.Banners;
using Container_App.Data.Repository.Banners;
using Container_App.Service.Dtos.Banner;
using Container_App.Service.Services.Cloudinarys;


namespace Container_App.Service.Services.Banners
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
