using CloudinaryDotNet.Actions;
using Container_App.Core.Model.KhachSans;
using Container_App.Core.Model.Provinces;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Provinces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Provinces
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
            try
            {
                return await _provinceRepository.GetProvinces();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get list province: {ex.Message}");

                return new List<Province>();
            }
        }
    }
}
