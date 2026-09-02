namespace Booking.Service.Dtos.LichSuVi;

public class LichSuViDto
{
    public long MaLichSu { get; set; }

    public Guid? MaDatPhong { get; set; }

    public decimal SoTien { get; set; }

    public string LoaiGiaoDich { get; set; }

    public string? NoiDung { get; set; }

    public DateTime NgayTao { get; set; } = DateTime.Now;
}