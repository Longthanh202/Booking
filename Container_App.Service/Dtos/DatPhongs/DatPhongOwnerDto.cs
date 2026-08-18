namespace Container_App.Service.Dtos.DatPhongs;

public class DatPhongOwnerDto
{
    public List<DatPhongDto> Data { get; set; } = new();
    public int TotalRow { get; set; }
    public int TotalPage { get; set; }
}

public class BookingHistory
{
    public List<DatPhongDto> Data { get; set; } = new();
    public int TotalRow { get; set; }
    public int TotalPage { get; set; }
}

public class DatPhongDto
{
    public Guid Id { get; set; }

    public string? TrangThai { get; set; }

    public string? ThanhToan { get; set; }

    public Guid? KhachSanId { get; set; }

    public string? TenKhachSan { get; set; }

    public DateTime? NgayTao { get; set; }
    public DateTime? NgayNhan { get; set; }
    public DateTime? NgayTra { get; set; }

    public List<ChiTietDatPhongDto> ChiTietDatPhongs { get; set; } = new();
}

public class ChiTietDatPhongDto
{
    public Guid Id { get; set; }

    public Guid? LoaiPhongId { get; set; }

    public string? TenLoaiPhong { get; set; }

    public decimal? Gia { get; set; }

    public List<PhongDto> Phongs { get; set; } = new();
}

public class PhongDto
{
    public Guid Id { get; set; }

    public string? SoPhong { get; set; }

    public string? TrangThai { get; set; }
}

public class DatPhongOwnerRequest
{
    public Guid khachSanId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}