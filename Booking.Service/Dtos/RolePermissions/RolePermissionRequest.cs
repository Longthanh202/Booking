using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.RolePermissions
{
    public class RolePermissionRequest
    {
        public Guid RoleId { get; set; }
        public List<RolePermissionDto> Permissions { get; set; }
    }
}
