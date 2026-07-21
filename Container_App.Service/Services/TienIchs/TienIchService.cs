using Container_App.Core.Model.TienIchs;
using Container_App.Data.Connection;
using Container_App.Data.Repository.TienIchs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Services.TienIchs
{
    public class TienIchService : ITienIchService
    {
        private readonly ITienIchRepository _tienIchRepository;
        public TienIchService(ITienIchRepository tienIchRepository)
        {
            _tienIchRepository = tienIchRepository;
        }

        public async Task<List<TienIch>> GetTienIchKhachSanByKhachSanId(Guid khachSanId)
        {
            try
            {
                return await _tienIchRepository.GetTienIchKhachSanByKhachSanId(khachSanId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when GetTienIchKhachSanByKhachSanId.");

                return new List<TienIch>();
            }
        }

        public async Task<TienIch> ThemTienIch(TienIch tienIch)
        {
            try
            {
                tienIch.Id = Guid.NewGuid();
                return await _tienIchRepository.ThemTienIch(tienIch);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message,
                "Error when Them Tien Ich.");

                return null;
            }
        }
    }
}
