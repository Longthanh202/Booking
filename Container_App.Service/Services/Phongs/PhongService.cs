using Container_App.Core.Model.Phongs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Phongs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Phongs
{
    public class PhongService : IPhongService
    {
        private readonly IPhongRepository _phongRepository;
        public PhongService(IPhongRepository phongRepository)
        {
           _phongRepository = phongRepository;
        }
        public async Task<Phong> TaoPhong(Phong p)
        {
            try
            {
                p.Id = Guid.NewGuid();
                return await _phongRepository.TaoPhong(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when create Phong.");

                return null;
            }
        }
    }
}
