using Booking.Core.Model.QuangCaos;

namespace Booking.Data.Repository.QuangCaos
{
    public interface IQuangCaoRepository
    {
        Task<List<GoiQuangCao>> GetActivePackages();
        Task<GoiQuangCao?> GetActivePackage(int id);
        Task<List<KhachSanQuangCao>> GetByOwner(Guid ownerId);
        Task<List<KhachSanQuangCao>> GetPendingApproval();
        Task<KhachSanQuangCao?> Create(KhachSanQuangCao campaign, Guid ownerId);
        Task<KhachSanQuangCao?> GetOwned(long id, Guid ownerId);
        Task<KhachSanQuangCao?> GetById(long id);
        Task<KhachSanQuangCao?> Update(KhachSanQuangCao campaign);
    }
}