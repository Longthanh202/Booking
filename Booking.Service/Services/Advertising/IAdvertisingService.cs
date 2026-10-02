using Booking.Core.Model.QuangCaos;
using Booking.Service.Dtos.Advertising;

namespace Booking.Service.Services.Advertising
{
    public interface IAdvertisingService
    {
        Task<List<GoiQuangCao>> GetPackages();
        Task<List<KhachSanQuangCao>> GetOwnerCampaigns(Guid ownerId);
        Task<KhachSanQuangCao?> CreateOrder(Guid ownerId, CreateAdvertisingRequest request);
        Task<KhachSanQuangCao?> ConfirmPayment(Guid ownerId, long id);
        Task<List<KhachSanQuangCao>> GetPendingApproval();
        Task<KhachSanQuangCao?> Approve(long id, ApproveAdvertisingRequest request);
    }
}