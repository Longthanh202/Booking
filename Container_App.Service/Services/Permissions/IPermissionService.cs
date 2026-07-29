using Container_App.Core.Model.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Data.Repository.Permissions
{
    public interface IPermissionService
    {
        Task<List<PermissionInfo>> GetListPermissionByUser(Guid userId);
    }
}
