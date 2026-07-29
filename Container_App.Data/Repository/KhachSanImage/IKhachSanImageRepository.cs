using Container_App.Core.Model.KhachSanImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.KhachSanImage
{
    public interface IKhachSanImageRepository
    {
        Task<List<KhachSanImages>> GetHotelImages(List<Guid> ids);
        Task<List<KhachSanImages>> GetListImageByKhachSanId(Guid khachSanId);
        Task InsertKhachSanImage(Guid khachSanId, List<string> Url);
    }
}
