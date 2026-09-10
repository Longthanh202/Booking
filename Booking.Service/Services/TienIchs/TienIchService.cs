using Booking.Common.Shared;
using Booking.Core.Model.TienIchs;
using Booking.Data;
using Booking.Data.Connection;
using Booking.Data.Repository.KhachSanTienIch;
using Booking.Data.Repository.TienIchs;
using Booking.Service.Dtos.TienIchs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Services.TienIchs
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
            return await _tienIchRepository.GetTienIchKhachSanByKhachSanId(khachSanId);
        }

        public async Task<int> ThemTienIch(List<TienIchRequest> tienIch)
        {
            if (tienIch == null || tienIch.Count == 0)
                return 0;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                foreach (var item in tienIch)
                {
                    var tienIchId = Guid.NewGuid();

                    var input = new TienIch
                    {
                        Id = tienIchId,
                        TenTienIch = item.TenTienIch,
                        Icon = item.Icon
                    };

                    await _tienIchRepository.ThemTienIch(input);

                    var ksti = new KhachSan_TienIch
                    {
                        KhachSanId = item.KhachSanId,
                        TienIchId = tienIchId
                    };

                    await _khachSanTienIchRepository.Tao(ksti);
                }

                await _unitOfWork.CommitAsync();

                return tienIch.Count;
            }
            catch (Exception ex)
            {
                FileLogger.Log(ex);

                await _unitOfWork.RollbackAsync();

                throw;
            }
        }
    }
}
