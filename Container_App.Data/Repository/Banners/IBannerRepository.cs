using Container_App.Core.Model.Banners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Banners
{
    public interface IBannerRepository
    {
        Task<Banner> InsertBanner(Banner banner);
        Task<List<Banner>> GetAllBanner(string keyword, int isActive, int startRow, int endRow);

        Task<List<Banner>> GetBannerIsActive();
    }
}
