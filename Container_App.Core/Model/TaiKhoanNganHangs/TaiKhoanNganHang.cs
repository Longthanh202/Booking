using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container_App.Core.Model.TaiKhoanNganHangs
{
    public class TaiKhoanNganHang
    {
        public Guid Id { get; set; }

        // Chỉ lưu UserId, không tạo FK
        public Guid UserId { get; set; }

        // Tên ngân hàng
        public string TenNganHang { get; set; } = string.Empty;

        // Số tài khoản
        public string SoTaiKhoan { get; set; } = string.Empty;

        // Chủ tài khoản
        public string ChuTaiKhoan { get; set; } = string.Empty;

        // Chi nhánh (nếu có)
        public string? ChiNhanh { get; set; }

        // Mã QR hoặc URL QR (nếu có)
        public string? QrCode { get; set; }

        public bool IsDefault { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
    }
}
