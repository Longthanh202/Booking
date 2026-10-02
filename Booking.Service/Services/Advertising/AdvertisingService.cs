using Booking.Common.Shared.Enum.QuangCao;
using Booking.Core.Model.QuangCaos;
using Booking.Data.Repository.QuangCaos;
using Booking.Service.Dtos.Advertising;

namespace Booking.Service.Services.Advertising
{
    public class AdvertisingService : IAdvertisingService
    {
        private readonly IQuangCaoRepository _repository;

        public AdvertisingService(IQuangCaoRepository repository)
        {
            _repository = repository;
        }

        public Task<List<GoiQuangCao>> GetPackages() => _repository.GetActivePackages();

        public Task<List<KhachSanQuangCao>> GetOwnerCampaigns(Guid ownerId) => _repository.GetByOwner(ownerId);

        public async Task<KhachSanQuangCao?> CreateOrder(Guid ownerId, CreateAdvertisingRequest request)
        {
            var package = await _repository.GetActivePackage(request.GoiQuangCaoId);
            if (package == null || request.KhachSanId == Guid.Empty)
            {
                return null;
            }

            return await _repository.Create(new KhachSanQuangCao
            {
                KhachSanId = request.KhachSanId,
                GoiQuanCaoId = package.Id,
                DiemUuTien = package.DiemUuTien,
                TrangThai = TrangThaiQuangCao.CHO_THANH_TOAN.ToString(),
                CreatedDate = DateTime.UtcNow
            }, ownerId);
        }

        public async Task<KhachSanQuangCao?> ConfirmPayment(Guid ownerId, long id)
        {
            var campaign = await _repository.GetOwned(id, ownerId);
            if (campaign == null || campaign.TrangThai != TrangThaiQuangCao.CHO_THANH_TOAN.ToString())
            {
                return null;
            }

            campaign.TrangThai = TrangThaiQuangCao.CHO_DUYET.ToString();
            return await _repository.Update(campaign);
        }

        public Task<List<KhachSanQuangCao>> GetPendingApproval() => _repository.GetPendingApproval();

        public async Task<KhachSanQuangCao?> Approve(long id, ApproveAdvertisingRequest request)
        {
            var campaign = await _repository.GetById(id);
            if (campaign == null || campaign.TrangThai != TrangThaiQuangCao.CHO_DUYET.ToString())
            {
                return null;
            }

            if (!request.Approved)
            {
                campaign.TrangThai = TrangThaiQuangCao.TU_CHOI.ToString();
                return await _repository.Update(campaign);
            }

            var days = campaign.GoiQuangCao?.SoNgay ?? 0;
            if (days <= 0)
            {
                return null;
            }

            campaign.NgayBatDau = DateTime.UtcNow;
            campaign.NgayKetThuc = campaign.NgayBatDau.Value.AddDays(days);
            campaign.TrangThai = TrangThaiQuangCao.DANG_HIEN_THI.ToString();
            return await _repository.Update(campaign);
        }
    }
}