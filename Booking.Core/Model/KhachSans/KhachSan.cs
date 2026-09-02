using Booking.Core.Model.ChiTraKhachSans;
using Booking.Core.Model.DanhGias;
using Booking.Core.Model.DatPhongs;
using Booking.Core.Model.HoaDonHoaHongs;
using Booking.Core.Model.HoaHongs;
using Booking.Core.Model.KhachSanImage;
using Booking.Core.Model.LoaiPhongs;
using Booking.Core.Model.Provinces;
using Booking.Core.Model.QuangCaos;
using Booking.Core.Model.TienIchs;
using Booking.Core.Model.ViKhachSans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Booking.Core.Model.KhachSans
{
    public class KhachSan
    {
        [Key]
        public Guid Id { get; set; }
        public Guid NguoiTao { get; set; }
        public string? TenKhachSan { get; set; }
        public string? MoTa { get; set; }
        public string? DiaChi { get; set; }
        public string? ThanhPho { get; set; }
        public double? ViDo { get; set; }
        public double? KinhDo { get; set; }
        public int? SoSao { get; set; }
        public TimeSpan? GioNhanPhong { get; set; } // TIME trong SQL ứng với TimeSpan
        public TimeSpan? GioTraPhong { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        public Province Province { get; set; }
        public virtual ICollection<KhachSanImages> KhachSanImages { get; set; } = new List<KhachSanImages>();
        public virtual ICollection<LoaiPhong> LoaiPhongs { get; set; } = new List<LoaiPhong>();
        public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
        public virtual ICollection<KhachSanQuangCao> KhachSanQuangCaos { get; set; } = new List<KhachSanQuangCao>();

        // Mối quan hệ Nhiều - Nhiều qua bảng trung gian
        public virtual ICollection<KhachSan_TienIch> KhachSan_TienIches { get; set; } = new List<KhachSan_TienIch>();

        public virtual ViKhachSan? ViKhachSan { get; set; }
        [JsonIgnore]
        public virtual ICollection<HoaHong> HoaHongs { get; set; } = new List<HoaHong>();

        public virtual ICollection<ChiTraKhachSan> ChiTraKhachSans { get; set; } = new List<ChiTraKhachSan>();

        public virtual ICollection<HoaDonHoaHong> HoaDonHoaHongs { get; set; } = new List<HoaDonHoaHong>();
    }
}
