USE [Stayro]
GO
/****** Object:  Table [dbo].[administrative_regions]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[administrative_regions](
	[id] [int] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[name_en] [nvarchar](255) NOT NULL,
	[code_name] [nvarchar](255) NULL,
	[code_name_en] [nvarchar](255) NULL,
 CONSTRAINT [administrative_regions_pkey] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[administrative_units]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[administrative_units](
	[id] [int] NOT NULL,
	[full_name] [nvarchar](255) NULL,
	[full_name_en] [nvarchar](255) NULL,
	[short_name] [nvarchar](255) NULL,
	[short_name_en] [nvarchar](255) NULL,
	[code_name] [nvarchar](255) NULL,
	[code_name_en] [nvarchar](255) NULL,
 CONSTRAINT [administrative_units_pkey] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Banner]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banner](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](500) NULL,
	[Subtitle] [nvarchar](1000) NULL,
	[Url] [nvarchar](1000) NULL,
	[IsActive] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTietChiTraKhachSan]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTietChiTraKhachSan](
	[MaChiTiet] [bigint] IDENTITY(1,1) NOT NULL,
	[MaChiTra] [bigint] NOT NULL,
	[MaDatPhong] [uniqueidentifier] NOT NULL,
	[SoTienThanhToan] [decimal](18, 2) NOT NULL,
	[SoTienHoaHong] [decimal](18, 2) NOT NULL,
	[SoTienThucNhan] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaChiTiet] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTietDatPhong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTietDatPhong](
	[Id] [uniqueidentifier] NOT NULL,
	[DatPhongId] [uniqueidentifier] NULL,
	[LoaiPhongId] [uniqueidentifier] NULL,
	[SoLuongPhong] [int] NULL,
	[GiaMoiDem] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTietHoaDonHoaHong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTietHoaDonHoaHong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HoaDonHoaHongId] [bigint] NOT NULL,
	[HoaHongId] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTraKhachSan]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTraKhachSan](
	[MaChiTra] [bigint] IDENTITY(1,1) NOT NULL,
	[MaKhachSan] [uniqueidentifier] NOT NULL,
	[MaTaiKhoanNganHang] [uniqueidentifier] NOT NULL,
	[SoTien] [decimal](18, 2) NOT NULL,
	[TrangThai] [nvarchar](100) NULL,
	[MaGiaoDich] [nvarchar](100) NULL,
	[GhiChu] [nvarchar](500) NULL,
	[NgayTao] [datetime] NOT NULL,
	[NgayChuyenTien] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaChiTra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DanhGia]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DanhGia](
	[Id] [uniqueidentifier] NOT NULL,
	[KhachSanId] [uniqueidentifier] NULL,
	[KhachHangId] [uniqueidentifier] NULL,
	[SoSao] [int] NULL,
	[NoiDung] [nvarchar](max) NULL,
	[NgayTao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatPhong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatPhong](
	[Id] [uniqueidentifier] NOT NULL,
	[KhachHangId] [uniqueidentifier] NULL,
	[KhachSanId] [uniqueidentifier] NULL,
	[NgayNhanPhong] [date] NULL,
	[NgayTraPhong] [date] NULL,
	[TongTien] [decimal](18, 2) NULL,
	[TrangThai] [nvarchar](50) NULL,
	[NgayTao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExternalLogins]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExternalLogins](
	[Id] [uniqueidentifier] NOT NULL,
	[UserLoginId] [uniqueidentifier] NOT NULL,
	[Provider] [nvarchar](50) NOT NULL,
	[ProviderKey] [nvarchar](256) NOT NULL,
	[Avatar] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GiaPhong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GiaPhong](
	[Id] [uniqueidentifier] NOT NULL,
	[LoaiPhongId] [uniqueidentifier] NOT NULL,
	[Gia] [decimal](18, 2) NOT NULL,
	[NgayBatDau] [datetime] NOT NULL,
	[NgayKetThuc] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[NgayTao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GoiQuangCao]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GoiQuangCao](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenGoi] [nvarchar](255) NULL,
	[SoNgay] [int] NULL,
	[GiaTien] [decimal](18, 2) NULL,
	[DiemUuTien] [int] NULL,
	[IsActive] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HoaDonHoaHong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HoaDonHoaHong](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhachSanId] [uniqueidentifier] NOT NULL,
	[TuNgay] [datetime2](7) NOT NULL,
	[DenNgay] [datetime2](7) NOT NULL,
	[TongTienHoaHong] [decimal](18, 2) NOT NULL,
	[TrangThai] [nvarchar](50) NOT NULL,
	[NgayTao] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HoaHong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HoaHong](
	[MaHoaHong] [bigint] IDENTITY(1,1) NOT NULL,
	[MaDatPhong] [uniqueidentifier] NOT NULL,
	[MaKhachSan] [uniqueidentifier] NOT NULL,
	[TyLeHoaHong] [decimal](5, 2) NOT NULL,
	[SoTienHoaHong] [decimal](18, 2) NOT NULL,
	[TrangThai] [nvarchar](100) NULL,
	[NgayTao] [datetime] NOT NULL,
	[NgayThu] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaHoaHong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhachSan]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachSan](
	[Id] [uniqueidentifier] NOT NULL,
	[NguoiTao] [uniqueidentifier] NOT NULL,
	[TenKhachSan] [nvarchar](255) NULL,
	[MoTa] [nvarchar](max) NULL,
	[DiaChi] [nvarchar](500) NULL,
	[ThanhPho] [nvarchar](100) NULL,
	[ViDo] [float] NULL,
	[KinhDo] [float] NULL,
	[SoSao] [int] NULL,
	[GioNhanPhong] [time](7) NULL,
	[GioTraPhong] [time](7) NULL,
	[TrangThai] [nvarchar](50) NULL,
	[NgayTao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhachSan_TienIch]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachSan_TienIch](
	[KhachSanId] [uniqueidentifier] NOT NULL,
	[TienIchId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[KhachSanId] ASC,
	[TienIchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhachSanImages]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachSanImages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhachSanId] [uniqueidentifier] NULL,
	[Url] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhachSanQuangCao]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachSanQuangCao](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KhachSanId] [uniqueidentifier] NULL,
	[GoiQuanCaoId] [int] NULL,
	[NgayBatDau] [datetime] NULL,
	[NgayKetThuc] [datetime] NULL,
	[DiemUuTien] [int] NULL,
	[TrangThai] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LichSuVi]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LichSuVi](
	[MaLichSu] [bigint] IDENTITY(1,1) NOT NULL,
	[MaVi] [bigint] NOT NULL,
	[MaDatPhong] [uniqueidentifier] NULL,
	[SoTien] [decimal](18, 2) NOT NULL,
	[LoaiGiaoDich] [nvarchar](100) NULL,
	[NoiDung] [nvarchar](500) NULL,
	[NgayTao] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaLichSu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiPhong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiPhong](
	[Id] [uniqueidentifier] NOT NULL,
	[KhachSanId] [uniqueidentifier] NULL,
	[TenLoaiPhong] [nvarchar](255) NULL,
	[SoKhachToiDa] [int] NULL,
	[KieuGiuong] [nvarchar](100) NULL,
	[MoTa] [nvarchar](max) NULL,
	[NgayTao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[Id] [uniqueidentifier] NOT NULL,
	[Action] [nvarchar](100) NULL,
	[CreateAt] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Phong]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Phong](
	[Id] [uniqueidentifier] NOT NULL,
	[LoaiPhongId] [uniqueidentifier] NULL,
	[SoPhong] [nvarchar](50) NULL,
	[Tang] [int] NULL,
	[TrangThai] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhongDat]    Script Date: 21/09/2026 10:17:16 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhongDat](
	[Id] [uniqueidentifier] NOT NULL,
	[ChiTietDatPhongId] [uniqueidentifier] NOT NULL,
	[PhongId] [uniqueidentifier] NOT NULL,
	[TrangThai] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[provinces]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[provinces](
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[name_en] [nvarchar](255) NULL,
	[full_name] [nvarchar](255) NOT NULL,
	[full_name_en] [nvarchar](255) NULL,
	[code_name] [nvarchar](255) NULL,
	[administrative_unit_id] [int] NULL,
 CONSTRAINT [provinces_pkey] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefreshToken]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshToken](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Token] [nvarchar](1000) NULL,
	[ExpiryDate] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [int] NULL,
	[FullName] [nvarchar](255) NULL,
	[RoleId] [uniqueidentifier] NULL,
	[RoleName] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Resources]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resources](
	[Id] [uniqueidentifier] NOT NULL,
	[ResourceName] [nvarchar](100) NULL,
	[Description] [nvarchar](255) NULL,
	[Icon] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ResourceName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NULL,
	[ResourceId] [uniqueidentifier] NULL,
	[PermissionId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[CreateAt] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaiKhoanNganHang]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaiKhoanNganHang](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[TenNganHang] [nvarchar](100) NOT NULL,
	[SoTaiKhoan] [nvarchar](50) NOT NULL,
	[ChuTaiKhoan] [nvarchar](150) NOT NULL,
	[ChiNhanh] [nvarchar](200) NULL,
	[QrCode] [nvarchar](max) NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThanhToan]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThanhToan](
	[Id] [uniqueidentifier] NOT NULL,
	[DatPhongId] [uniqueidentifier] NULL,
	[PhuongThuc] [nvarchar](50) NULL,
	[SoTien] [decimal](18, 2) NULL,
	[TrangThai] [nvarchar](50) NULL,
	[ThoiGianThanhToan] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TienIch]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TienIch](
	[Id] [uniqueidentifier] NOT NULL,
	[TenTienIch] [nvarchar](100) NULL,
	[Icon] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogin]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogin](
	[Id] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](100) NULL,
	[Password] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[Id] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](255) NULL,
	[Phone] [nvarchar](15) NULL,
	[Email] [nvarchar](255) NULL,
	[Address] [nvarchar](500) NULL,
	[IsDel] [int] NULL,
	[CreateAt] [datetime] NULL,
	[CreateBy] [uniqueidentifier] NULL,
	[UserLoginId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[RoleId] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ViKhachSan]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ViKhachSan](
	[MaVi] [bigint] IDENTITY(1,1) NOT NULL,
	[MaKhachSan] [uniqueidentifier] NOT NULL,
	[SoDu] [decimal](18, 2) NOT NULL,
	[SoDuTamGiu] [decimal](18, 2) NOT NULL,
	[NgayCapNhat] [datetime] NOT NULL,
	[TrangThai] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaVi] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MaKhachSan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[wards]    Script Date: 21/09/2026 10:17:17 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[wards](
	[code] [nvarchar](20) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[name_en] [nvarchar](255) NULL,
	[full_name] [nvarchar](255) NULL,
	[full_name_en] [nvarchar](255) NULL,
	[code_name] [nvarchar](255) NULL,
	[province_code] [nvarchar](20) NULL,
	[administrative_unit_id] [int] NULL,
 CONSTRAINT [wards_pkey] PRIMARY KEY CLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ChiTietDatPhong] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[ChiTraKhachSan] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[DanhGia] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[DanhGia] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[DatPhong] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[DatPhong] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[ExternalLogins] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ExternalLogins] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[GiaPhong] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[GiaPhong] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[GiaPhong] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[HoaDonHoaHong] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[HoaHong] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[KhachSan] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[KhachSan] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[LichSuVi] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[LoaiPhong] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[LoaiPhong] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[Phong] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[PhongDat] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[TaiKhoanNganHang] ADD  DEFAULT ((1)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[TaiKhoanNganHang] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[TienIch] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[ViKhachSan] ADD  DEFAULT ((0)) FOR [SoDu]
GO
ALTER TABLE [dbo].[ViKhachSan] ADD  DEFAULT ((0)) FOR [SoDuTamGiu]
GO
ALTER TABLE [dbo].[ViKhachSan] ADD  DEFAULT (getdate()) FOR [NgayCapNhat]
GO
ALTER TABLE [dbo].[ChiTietChiTraKhachSan]  WITH CHECK ADD FOREIGN KEY([MaChiTra])
REFERENCES [dbo].[ChiTraKhachSan] ([MaChiTra])
GO
ALTER TABLE [dbo].[ChiTietChiTraKhachSan]  WITH CHECK ADD FOREIGN KEY([MaDatPhong])
REFERENCES [dbo].[DatPhong] ([Id])
GO
ALTER TABLE [dbo].[ChiTietDatPhong]  WITH CHECK ADD  CONSTRAINT [FK_DatPhong_ChiTietDatPhong] FOREIGN KEY([DatPhongId])
REFERENCES [dbo].[DatPhong] ([Id])
GO
ALTER TABLE [dbo].[ChiTietDatPhong] CHECK CONSTRAINT [FK_DatPhong_ChiTietDatPhong]
GO
ALTER TABLE [dbo].[ChiTietHoaDonHoaHong]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietHoaDonHoaHong_HoaDon] FOREIGN KEY([HoaDonHoaHongId])
REFERENCES [dbo].[HoaDonHoaHong] ([Id])
GO
ALTER TABLE [dbo].[ChiTietHoaDonHoaHong] CHECK CONSTRAINT [FK_ChiTietHoaDonHoaHong_HoaDon]
GO
ALTER TABLE [dbo].[ChiTietHoaDonHoaHong]  WITH CHECK ADD  CONSTRAINT [FK_ChiTietHoaDonHoaHong_HoaHong] FOREIGN KEY([HoaHongId])
REFERENCES [dbo].[HoaHong] ([MaHoaHong])
GO
ALTER TABLE [dbo].[ChiTietHoaDonHoaHong] CHECK CONSTRAINT [FK_ChiTietHoaDonHoaHong_HoaHong]
GO
ALTER TABLE [dbo].[ChiTraKhachSan]  WITH CHECK ADD  CONSTRAINT [FK_ChiTraKhachSan_KhachSan] FOREIGN KEY([MaKhachSan])
REFERENCES [dbo].[KhachSan] ([Id])
GO
ALTER TABLE [dbo].[ChiTraKhachSan] CHECK CONSTRAINT [FK_ChiTraKhachSan_KhachSan]
GO
ALTER TABLE [dbo].[ChiTraKhachSan]  WITH CHECK ADD  CONSTRAINT [FK_ChiTraKhachSan_TaiKhoan] FOREIGN KEY([MaTaiKhoanNganHang])
REFERENCES [dbo].[TaiKhoanNganHang] ([Id])
GO
ALTER TABLE [dbo].[ChiTraKhachSan] CHECK CONSTRAINT [FK_ChiTraKhachSan_TaiKhoan]
GO
ALTER TABLE [dbo].[ExternalLogins]  WITH CHECK ADD  CONSTRAINT [FK_ExternalLogins_UserLogin] FOREIGN KEY([UserLoginId])
REFERENCES [dbo].[UserLogin] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExternalLogins] CHECK CONSTRAINT [FK_ExternalLogins_UserLogin]
GO
ALTER TABLE [dbo].[HoaDonHoaHong]  WITH CHECK ADD  CONSTRAINT [FK_HoaDonHoaHong_KhachSan] FOREIGN KEY([KhachSanId])
REFERENCES [dbo].[KhachSan] ([Id])
GO
ALTER TABLE [dbo].[HoaDonHoaHong] CHECK CONSTRAINT [FK_HoaDonHoaHong_KhachSan]
GO
ALTER TABLE [dbo].[HoaHong]  WITH CHECK ADD  CONSTRAINT [FK_HoaHong_DatPhong] FOREIGN KEY([MaDatPhong])
REFERENCES [dbo].[DatPhong] ([Id])
GO
ALTER TABLE [dbo].[HoaHong] CHECK CONSTRAINT [FK_HoaHong_DatPhong]
GO
ALTER TABLE [dbo].[HoaHong]  WITH CHECK ADD  CONSTRAINT [FK_HoaHong_KhachSan] FOREIGN KEY([MaKhachSan])
REFERENCES [dbo].[KhachSan] ([Id])
GO
ALTER TABLE [dbo].[HoaHong] CHECK CONSTRAINT [FK_HoaHong_KhachSan]
GO
ALTER TABLE [dbo].[LichSuVi]  WITH CHECK ADD  CONSTRAINT [FK_LichSuVi_DatPhong] FOREIGN KEY([MaDatPhong])
REFERENCES [dbo].[DatPhong] ([Id])
GO
ALTER TABLE [dbo].[LichSuVi] CHECK CONSTRAINT [FK_LichSuVi_DatPhong]
GO
ALTER TABLE [dbo].[LichSuVi]  WITH CHECK ADD  CONSTRAINT [FK_LichSuVi_Vi] FOREIGN KEY([MaVi])
REFERENCES [dbo].[ViKhachSan] ([MaVi])
GO
ALTER TABLE [dbo].[LichSuVi] CHECK CONSTRAINT [FK_LichSuVi_Vi]
GO
ALTER TABLE [dbo].[PhongDat]  WITH CHECK ADD  CONSTRAINT [FK_PhongDat_ChiTietDatPhong] FOREIGN KEY([ChiTietDatPhongId])
REFERENCES [dbo].[ChiTietDatPhong] ([Id])
GO
ALTER TABLE [dbo].[PhongDat] CHECK CONSTRAINT [FK_PhongDat_ChiTietDatPhong]
GO
ALTER TABLE [dbo].[PhongDat]  WITH CHECK ADD  CONSTRAINT [FK_PhongDat_Phong] FOREIGN KEY([PhongId])
REFERENCES [dbo].[Phong] ([Id])
GO
ALTER TABLE [dbo].[PhongDat] CHECK CONSTRAINT [FK_PhongDat_Phong]
GO
ALTER TABLE [dbo].[provinces]  WITH CHECK ADD  CONSTRAINT [provinces_administrative_unit_id_fkey] FOREIGN KEY([administrative_unit_id])
REFERENCES [dbo].[administrative_units] ([id])
GO
ALTER TABLE [dbo].[provinces] CHECK CONSTRAINT [provinces_administrative_unit_id_fkey]
GO
ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD  CONSTRAINT [FK_DatPhong_ThanhToan] FOREIGN KEY([DatPhongId])
REFERENCES [dbo].[DatPhong] ([Id])
GO
ALTER TABLE [dbo].[ThanhToan] CHECK CONSTRAINT [FK_DatPhong_ThanhToan]
GO
ALTER TABLE [dbo].[ViKhachSan]  WITH CHECK ADD  CONSTRAINT [FK_ViKhachSan_KhachSan] FOREIGN KEY([MaKhachSan])
REFERENCES [dbo].[KhachSan] ([Id])
GO
ALTER TABLE [dbo].[ViKhachSan] CHECK CONSTRAINT [FK_ViKhachSan_KhachSan]
GO
ALTER TABLE [dbo].[wards]  WITH CHECK ADD  CONSTRAINT [wards_administrative_unit_id_fkey] FOREIGN KEY([administrative_unit_id])
REFERENCES [dbo].[administrative_units] ([id])
GO
ALTER TABLE [dbo].[wards] CHECK CONSTRAINT [wards_administrative_unit_id_fkey]
GO
ALTER TABLE [dbo].[wards]  WITH CHECK ADD  CONSTRAINT [wards_province_code_fkey] FOREIGN KEY([province_code])
REFERENCES [dbo].[provinces] ([code])
GO
ALTER TABLE [dbo].[wards] CHECK CONSTRAINT [wards_province_code_fkey]
GO


TỔNG HỢP CHỨC NĂNG HỆ THỐNG BOOKING
Customer – Owner – Admin
1. Tổng quan vai trò
Hệ thống booking được chia thành 3 vai trò chính. Customer sử dụng hệ thống để tìm kiếm và đặt dịch vụ; Owner quản lý các property/resource thuộc sở hữu của mình; Admin quản trị toàn bộ hệ thống, người dùng, property, booking, thanh toán và cấu hình.
Vai trò	Mô tả
Customer	Người sử dụng dịch vụ và thực hiện booking.
Owner	Chủ sở hữu property/resource; chỉ quản lý dữ liệu thuộc phạm vi của mình.
Admin	Quản trị viên toàn hệ thống, có quyền quản lý và kiểm soát dữ liệu trên toàn hệ thống.
2. Chức năng Customer
Tài khoản
•	Đăng ký tài khoản.
•	Đăng nhập / đăng xuất.
•	Quên và đặt lại mật khẩu.
•	Cập nhật thông tin cá nhân.
•	Quản lý thông tin liên hệ.
Tìm kiếm và khám phá
•	Tìm kiếm property/dịch vụ.
•	Lọc theo địa điểm, giá, loại hình, tiện ích và đánh giá.
•	Sắp xếp kết quả.
•	Xem danh sách và chi tiết property.
•	Xem hình ảnh, tiện ích, chính sách và thông tin giá.
Booking
•	Kiểm tra tình trạng còn trống.
•	Chọn ngày/giờ và resource/phòng.
•	Tạo booking.
•	Xem chi tiết booking.
•	Theo dõi trạng thái booking.
•	Hủy booking theo chính sách.
•	Xem lịch sử booking.
Thanh toán
•	Chọn phương thức thanh toán.
•	Thanh toán booking.
•	Xem trạng thái giao dịch.
•	Nhận thông tin hoàn tiền nếu có.
Review
•	Đánh giá property/dịch vụ sau khi sử dụng.
•	Chấm điểm và viết nhận xét.
•	Xem các đánh giá của mình.
Thông báo
•	Nhận thông báo booking thành công.
•	Nhận thông báo thanh toán.
•	Nhận thông báo thay đổi hoặc hủy booking.
•	Nhận thông báo khuyến mãi và các thông tin liên quan.
3. Chức năng Owner
Dashboard
•	Tổng số booking.
•	Booking hôm nay và booking sắp tới.
•	Booking đã hoàn thành / đã hủy.
•	Doanh thu theo ngày, tháng, năm.
•	Tỷ lệ sử dụng resource/phòng.
Quản lý Property
•	Tạo property.
•	Cập nhật thông tin property.
•	Quản lý mô tả, hình ảnh, địa chỉ và tiện ích.
•	Thiết lập chính sách và giờ hoạt động/check-in/check-out.
•	Theo dõi trạng thái duyệt của property.
Quản lý Room / Resource
•	Thêm, sửa, xóa resource/phòng.
•	Thiết lập loại resource, sức chứa và giá.
•	Quản lý hình ảnh và tiện ích.
•	Quản lý trạng thái Available, Occupied, Maintenance, Blocked.
•	Quản lý availability.
Quản lý giá và khuyến mãi
•	Thiết lập giá cơ bản.
•	Thiết lập giá cuối tuần, ngày lễ và theo mùa.
•	Tạo promotion/coupon cho property của mình.
•	Thiết lập điều kiện giảm giá.
Quản lý Booking
•	Xem booking thuộc property của mình.
•	Xác nhận hoặc từ chối booking khi nghiệp vụ cho phép.
•	Hủy booking theo chính sách.
•	Check-in / Check-out.
•	Xem thông tin khách hàng liên quan đến booking.
Calendar
•	Xem lịch booking theo ngày/tuần/tháng.
•	Theo dõi resource/phòng còn trống hoặc đã được đặt.
•	Đánh dấu thời gian/resource không khả dụng.
Customer
•	Xem khách hàng đã booking property của mình.
•	Xem lịch sử booking liên quan.
•	Xem thông tin cần thiết phục vụ booking.
Doanh thu và báo cáo
•	Xem doanh thu theo khoảng thời gian.
•	Xem số lượng booking.
•	Xem tỷ lệ hủy.
•	Theo dõi phí nền tảng, refund và doanh thu thực nhận nếu hệ thống có marketplace.
Review
•	Xem review của khách về property.
•	Phản hồi review.
•	Báo cáo review vi phạm.
Staff
•	Tạo và quản lý nhân viên thuộc property nếu hệ thống hỗ trợ.
•	Phân quyền cho Manager, Receptionist hoặc Staff.
•	Giới hạn quyền nhân viên theo property.
Thông báo
•	Nhận thông báo booking mới.
•	Nhận thông báo booking bị hủy.
•	Nhận thông báo thanh toán/review.
•	Nhận thông báo property được duyệt hoặc từ chối.
4. Chức năng Admin
Dashboard
•	Tổng số Customer, Owner và Property.
•	Tổng số booking.
•	Tổng doanh thu.
•	Doanh thu/phí nền tảng.
•	Các chỉ số vận hành toàn hệ thống.
User Management
•	Xem danh sách người dùng.
•	Tạo, cập nhật và khóa/mở khóa tài khoản.
•	Quản lý Customer, Owner, Staff và Admin.
•	Quản lý trạng thái tài khoản.
Owner Management
•	Xem danh sách Owner.
•	Duyệt hoặc từ chối Owner.
•	Khóa/mở khóa Owner.
•	Xem property, booking và doanh thu của Owner.
Property Management
•	Xem tất cả property.
•	Duyệt hoặc từ chối property.
•	Cập nhật/trạng thái property theo chính sách hệ thống.
•	Ẩn, khóa hoặc gỡ property vi phạm.
•	Quản lý property theo Owner.
Booking Management
•	Xem toàn bộ booking.
•	Tìm kiếm và lọc booking.
•	Theo dõi trạng thái booking.
•	Hỗ trợ xử lý các trường hợp booking đặc biệt.
•	Can thiệp booking theo chính sách và quyền được cấp.
Payment & Refund
•	Xem toàn bộ giao dịch.
•	Theo dõi payment thành công/thất bại.
•	Quản lý refund.
•	Theo dõi commission/platform fee.
•	Quản lý payout cho Owner nếu hệ thống hỗ trợ.
Category & Amenity
•	Quản lý category/property type.
•	Quản lý danh sách tiện ích dùng chung.
•	Owner chọn các amenity do hệ thống cung cấp.
Promotion
•	Quản lý promotion toàn hệ thống.
•	Quản lý coupon.
•	Thiết lập chương trình khuyến mãi cấp hệ thống.
Review
•	Xem review toàn hệ thống.
•	Xử lý review vi phạm.
•	Ẩn/xóa review theo chính sách.
Role & Permission
•	Quản lý Role.
•	Quản lý Permission.
•	Gán Role cho User.
•	Gán Permission cho Role.
•	Kiểm soát quyền truy cập các chức năng.
Reports
•	Báo cáo doanh thu.
•	Báo cáo booking.
•	Báo cáo Customer và Owner.
•	Báo cáo Property.
•	Báo cáo hủy booking và refund.
•	Báo cáo commission/phí nền tảng.
System Settings
•	Cấu hình booking.
•	Cấu hình chính sách hủy.
•	Cấu hình thanh toán.
•	Cấu hình commission.
•	Cấu hình email/notification.
•	Cấu hình các tham số hệ thống.
Audit Log
•	Ghi nhận lịch sử đăng nhập và thao tác quan trọng.
•	Theo dõi thay đổi dữ liệu.
•	Theo dõi thao tác liên quan đến quyền, payment, refund và booking.
•	Phục vụ kiểm tra và truy vết.
5. Ma trận quyền tổng quát
Chức năng	Customer	Owner	Admin
Đặt booking	Có	Tùy nghiệp vụ	Có
Xem booking của mình	Có	Có	Có
Quản lý Property	Không	Property của mình	Tất cả
Quản lý Room/Resource	Không	Resource của mình	Tất cả
Quản lý User	Không	Không	Tất cả
Quản lý Owner	Không	Không	Tất cả
Duyệt Owner	Không	Không	Có
Duyệt Property	Không	Không	Có
Quản lý giá	Không	Property của mình	Tất cả
Promotion	Sử dụng	Property của mình	Toàn hệ thống
Payment	Thanh toán	Theo dõi liên quan	Toàn hệ thống
Refund	Yêu cầu	Hỗ trợ theo nghiệp vụ	Xử lý
Review	Tạo	Xem/Reply	Quản lý toàn hệ thống
Role & Permission	Không	Không	Có
Audit Log	Không	Có thể giới hạn Own	Toàn hệ thống
System Settings	Không	Không	Có
6. Nguyên tắc phân quyền đề xuất
•	Frontend React chỉ nên dùng để ẩn/hiện menu và chặn navigation; không được coi đây là lớp bảo mật chính.
•	Backend .NET API phải kiểm tra quyền cho mọi API quan trọng.
•	Owner chỉ được truy cập dữ liệu thuộc property/resource của mình.
•	Nên kết hợp RBAC (Role + Permission) với kiểm tra phạm vi dữ liệu (Resource/Owner-based Authorization).
•	Admin có phạm vi toàn hệ thống; Owner có phạm vi Own; Customer chủ yếu có phạm vi dữ liệu của chính mình.
•	Các thao tác quan trọng như xóa dữ liệu, refund, thay đổi quyền và khóa tài khoản nên được ghi Audit Log.
7. Gợi ý cấu trúc menu
CUSTOMER
•	Home
•	Search
•	Property Detail
•	My Bookings
•	Favorites
•	Payments
•	Reviews
•	Notifications
•	Profile
OWNER
•	Dashboard
•	My Properties
•	Rooms/Resources
•	Calendar
•	Bookings
•	Customers
•	Promotions
•	Reviews
•	Revenue
•	Staff
•	Settings
ADMIN
•	Dashboard
•	Users
•	Owners
•	Properties
•	Bookings
•	Payments
•	Refunds
•	Categories
•	Amenities
•	Promotions
•	Reviews
•	Reports
•	Roles & Permissions
•	Audit Logs
•	Settings
8. Mô hình dữ liệu quyền ở mức khái quát
User → UserRole → Role → RolePermission → Permission. Đối với Owner, nên có quan hệ Owner → Property → Room/Resource 
