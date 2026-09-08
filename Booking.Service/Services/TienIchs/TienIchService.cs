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
                throw;
            }
        }
    }
}
