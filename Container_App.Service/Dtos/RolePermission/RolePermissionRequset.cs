using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Service.Dtos.RolePermission
{
    public class RolePermissionRequset
    {
        public Guid RoleId { get; set; }
        public List<RolePermissionDto> Permissions { get; set; }
    }
}
