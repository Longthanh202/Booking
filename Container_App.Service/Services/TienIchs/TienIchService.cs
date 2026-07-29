using Container_App.Core.Model.TienIchs;
using Container_App.Data;
using Container_App.Data.Connection;
using Container_App.Data.Repository.KhachSanTienIch;
using Container_App.Data.Repository.TienIchs;
using Container_App.Service.Dtos.TienIchs;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKhachSanTienIchRepository _khachSanTienIchRepository;
        public TienIchService(ITienIchRepository tienIchRepository,
            IUnitOfWork unitOfWork, 
            IKhachSanTienIchRepository khachSanTienIchRepository)
        {
            _tienIchRepository = tienIchRepository;
            _unitOfWork = unitOfWork;
            _khachSanTienIchRepository = khachSanTienIchRepository ;
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

        public async Task<TienIch> ThemTienIch(TienIchRequest tienIch)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                Guid tienIchId = Guid.NewGuid();

                TienIch input = new TienIch
                {
                    Id = tienIchId,
                    TenTienIch = tienIch.TenTienIch,
                    Icon = tienIch.Icon
                };

                var result = await _tienIchRepository.ThemTienIch(input);

                KhachSan_TienIch ksti = new KhachSan_TienIch
                {
                    KhachSanId = tienIch.KhachSanId,
                    TienIchId = tienIchId
                };

                await _khachSanTienIchRepository.Tao(ksti);

                await _unitOfWork.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();

                Console.WriteLine(ex);

                return null;
            }
        }
    }
}
