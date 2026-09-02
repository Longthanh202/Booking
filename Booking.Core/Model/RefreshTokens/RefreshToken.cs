using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Core.Model.RefreshTokens
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Token { get; set; }
        public int? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }
        public string? FullName { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
