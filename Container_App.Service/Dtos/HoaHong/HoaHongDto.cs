namespace Container_App.Service.Dtos.HoaHong;

public class HoaHongDto
{
    public long MaHoaHong { get; set; }

    public Guid MaDatPhong { get; set; }

    public Guid MaKhachSan { get; set; }
    
    public string TenKhachSan { get; set; }

    public decimal TyLeHoaHong { get; set; }

    public decimal SoTienHoaHong { get; set; }

    public string? TrangThai { get; set; }

    public DateTime NgayTao { get; set; } = DateTime.Now;

    public DateTime? NgayThu { get; set; }
}