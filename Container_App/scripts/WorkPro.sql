use workpro

create table UserLogin
(
	Id uniqueidentifier primary key,
	Username nvarchar(100),
	[Password] nvarchar(100)
)

create table UserProfile
(
	Id uniqueidentifier primary key,
	FullName nvarchar(255),
	Phone nvarchar(15),
	Email nvarchar(255),
	[Address] nvarchar(500),
	IsDel int,
	CreateAt datetime,
	CreateBy uniqueidentifier,
	UserLoginId uniqueidentifier
)


create table Roles
(
	Id uniqueidentifier primary key,
	RoleName nvarchar(255),
	[Description] nvarchar(255),
	CreateAt datetime,
	CreateBy uniqueidentifier,
)

CREATE TABLE Resources (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ResourceName NVARCHAR(100) UNIQUE,
    [Description] NVARCHAR(255)
);


create table [Permissions]
(
	Id uniqueidentifier primary key,
	[Action] nvarchar(100),
	CreateAt datetime,
	CreateBy uniqueidentifier
)

create table RolePermissions
(
	Id uniqueidentifier primary key,
	RoleId uniqueidentifier,
	ResourceId UNIQUEIDENTIFIER,
	PermissionId uniqueidentifier
)

create table UserRoles
(
	Id uniqueidentifier primary key,
	UserId uniqueidentifier,
	RoleId uniqueidentifier
)
go

CREATE TABLE KhachSan (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    NguoiTao UNIQUEIDENTIFIER NOT NULL,
    TenKhachSan NVARCHAR(255),
    MoTa NVARCHAR(MAX),
    DiaChi NVARCHAR(500),
    ThanhPho NVARCHAR(100),
    ViDo FLOAT,
    KinhDo FLOAT,
    SoSao INT,
    GioNhanPhong TIME,
    GioTraPhong TIME,
    TrangThai NVARCHAR(50), -- NHAP | HOAT_DONG | KHOA
    NgayTao DATETIME DEFAULT GETDATE(),
);

go
Create table KhachSanImages
(
	Id bigint primary key identity,
	KhachSanId uniqueidentifier,
	Url nvarchar(1000)
)


CREATE TABLE LoaiPhong (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    KhachSanId UNIQUEIDENTIFIER,
    TenLoaiPhong NVARCHAR(255),
    SoKhachToiDa INT,
    KieuGiuong NVARCHAR(100),
    MoTa NVARCHAR(MAX),
    NgayTao DATETIME DEFAULT GETDATE(),
);

CREATE TABLE Phong (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    LoaiPhongId UNIQUEIDENTIFIER,
    SoPhong NVARCHAR(50),
    Tang INT,
    TrangThai NVARCHAR(50), -- SAN_SANG | BAO_TRI
);

CREATE TABLE GiaPhong (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    LoaiPhongId UNIQUEIDENTIFIER,
    Ngay DATE,
    Gia DECIMAL(18,2),
    SoPhongCon INT,
);

CREATE TABLE DatPhong (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    KhachHangId UNIQUEIDENTIFIER,
    KhachSanId UNIQUEIDENTIFIER,
    NgayNhanPhong DATE,
    NgayTraPhong DATE,
    TongTien DECIMAL(18,2),
    TrangThai NVARCHAR(50), 
    -- CHO_THANH_TOAN | DA_XAC_NHAN | DA_HUY | HOAN_THANH
    NgayTao DATETIME DEFAULT GETDATE(),
);

CREATE TABLE ChiTietDatPhong (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    DatPhongId UNIQUEIDENTIFIER,
    LoaiPhongId UNIQUEIDENTIFIER,
    SoLuongPhong INT,
    GiaMoiDem DECIMAL(18,2)
);

CREATE TABLE ThanhToan (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    DatPhongId UNIQUEIDENTIFIER,
    PhuongThuc NVARCHAR(50), -- VNPAY | MOMO | THE
    SoTien DECIMAL(18,2),
    TrangThai NVARCHAR(50), -- THANH_CONG | THAT_BAI | HOAN_TIEN
    ThoiGianThanhToan DATETIME
);

CREATE TABLE DanhGia (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    KhachSanId UNIQUEIDENTIFIER,
    KhachHangId UNIQUEIDENTIFIER,
    SoSao INT, -- 1-5
    NoiDung NVARCHAR(MAX),
    NgayTao DATETIME DEFAULT GETDATE(),
);

CREATE TABLE TienIch (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    TenTienIch NVARCHAR(100),
    Icon NVARCHAR(100)
);

CREATE TABLE KhachSan_TienIch (
    KhachSanId UNIQUEIDENTIFIER,
    TienIchId UNIQUEIDENTIFIER,
    PRIMARY KEY (KhachSanId, TienIchId)
);

alter table Resources
add Icon nvarchar(100)

create table RefreshToken
(
	Id bigint primary key identity,
	UserId uniqueidentifier,
	Token nvarchar(1000),
	ExpiryDate int,
	CreatedDate datetime,
	Status int,
	FullName nvarchar(255),
	RoleId uniqueidentifier,
	RoleName nvarchar(255)
)

create table Banner
(
	Id bigint primary key identity,
	Title nvarchar(500),
	Subtitle nvarchar(1000),
	Url nvarchar(1000),
	IsActive int,
	CreatedDate datetime
)
go

create table GoiQuangCao
(
	Id int primary key identity,
	TenGoi nvarchar(255),
	SoNgay int,
	GiaTien decimal(18, 2),
	DiemUuTien int,
	IsActive int,
	CreatedDate datetime
)


create table KhachSanQuangCao
(
	Id bigint primary key identity,
	KhachSanId uniqueidentifier,
	GoiQuanCaoId int,
	NgayBatDau datetime,
	NgayKetThuc datetime,
	DiemUuTien int,
	TrangThai nvarchar(50),
	CreatedDate datetime
)


alter table Phong
add KhachSanId uniqueidentifier


SELECT *
FROM KhachSan ks
LEFT JOIN KhachSanQuangCao qc
    ON ks.id = qc.KhachSanId
    AND qc.TrangThai = 'active'
    AND GETDATE() BETWEEN qc.NgayBatDau AND qc.NgayKetThuc
ORDER BY 
    ISNULL(qc.DiemUuTien, 0) DESC
