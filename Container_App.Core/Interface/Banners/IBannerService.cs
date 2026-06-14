using Container_App.Core.Model.Banners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Interface.Banners
{
    public interface IBannerService
    {
        Task<int> InsertBanner(Banner banner);
        Task<IEnumerable<Banner>> GetAllBanner(string keyword, int isActive, int startRow, int endRow);

        Task<IEnumerable<Banner>> Get3BannerNew();
    }
}
