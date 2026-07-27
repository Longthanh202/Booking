using Container_App.Core.Model.Provinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Provinces
{
    public interface IProvinceService
    {
        Task<List<Province>> GetProvinces();
    }
}
