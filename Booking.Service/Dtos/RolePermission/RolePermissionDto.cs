using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Service.Dtos.RolePermission
{
    public class RolePermissionDto
    {
        public Guid ResourceId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
