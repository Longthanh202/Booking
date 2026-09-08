using CloudinaryDotNet.Actions;
using Booking.Core.Model.KhachSans;
using Booking.Core.Model.Provinces;
using Booking.Data.Connection;
using Booking.Data.Repository.Provinces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.Provinces
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;
        public ProvinceService(IProvinceRepository provinceRepository)
        {
           _provinceRepository = provinceRepository;
        }
        public async Task<List<Province>> GetProvinces()
        {
            return await _provinceRepository.GetProvinces();
        }
    }
}
