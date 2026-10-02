IF OBJECT_ID(N'dbo.RoomAvailabilityBlocks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoomAvailabilityBlocks
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_RoomAvailabilityBlocks PRIMARY KEY,
        RoomId UNIQUEIDENTIFIER NOT NULL,
        StartAt DATETIME2 NOT NULL,
        EndAt DATETIME2 NOT NULL,
        Reason NVARCHAR(300) NULL,
        CreatedBy UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT FK_RoomAvailabilityBlocks_Phong FOREIGN KEY (RoomId)
            REFERENCES dbo.Phong(Id) ON DELETE CASCADE,
        CONSTRAINT CK_RoomAvailabilityBlocks_Range CHECK (StartAt < EndAt)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_RoomAvailabilityBlocks_RoomId_StartAt_EndAt'
      AND object_id = OBJECT_ID(N'dbo.RoomAvailabilityBlocks')
)
BEGIN
    CREATE INDEX IX_RoomAvailabilityBlocks_RoomId_StartAt_EndAt
        ON dbo.RoomAvailabilityBlocks(RoomId, StartAt, EndAt);
END;
GO

IF OBJECT_ID(N'dbo.HotelPromotions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HotelPromotions
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_HotelPromotions PRIMARY KEY,
        HotelId UNIQUEIDENTIFIER NOT NULL,
        Code NVARCHAR(40) NOT NULL,
        Name NVARCHAR(150) NOT NULL,
        DiscountType NVARCHAR(10) NOT NULL,
        DiscountValue DECIMAL(18,2) NOT NULL,
        MinBookingAmount DECIMAL(18,2) NULL,
        MaxDiscountAmount DECIMAL(18,2) NULL,
        StartsAt DATETIME2 NOT NULL,
        EndsAt DATETIME2 NOT NULL,
        IsActive BIT NOT NULL,
        CreatedBy UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT FK_HotelPromotions_KhachSan FOREIGN KEY (HotelId)
            REFERENCES dbo.KhachSan(Id) ON DELETE CASCADE,
        CONSTRAINT CK_HotelPromotions_DiscountType CHECK (DiscountType IN (N'PERCENT', N'FIXED')),
        CONSTRAINT CK_HotelPromotions_Range CHECK (StartsAt < EndsAt),
        CONSTRAINT CK_HotelPromotions_DiscountValue CHECK (DiscountValue > 0)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_HotelPromotions_HotelId_Code'
      AND object_id = OBJECT_ID(N'dbo.HotelPromotions')
)
BEGIN
    CREATE UNIQUE INDEX UX_HotelPromotions_HotelId_Code
        ON dbo.HotelPromotions(HotelId, Code);
END;
GO

IF COL_LENGTH(N'dbo.DatPhong', N'SoTienGiam') IS NULL
BEGIN
    ALTER TABLE dbo.DatPhong
        ADD SoTienGiam DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_DatPhong_SoTienGiam DEFAULT (0);
END;
GO

IF COL_LENGTH(N'dbo.DatPhong', N'MaKhuyenMai') IS NULL
BEGIN
    ALTER TABLE dbo.DatPhong ADD MaKhuyenMai NVARCHAR(40) NULL;
END;
GO

IF COL_LENGTH(N'dbo.DanhGia', N'OwnerResponse') IS NULL
    ALTER TABLE dbo.DanhGia ADD OwnerResponse NVARCHAR(2000) NULL;
GO

IF COL_LENGTH(N'dbo.DanhGia', N'OwnerResponseAt') IS NULL
    ALTER TABLE dbo.DanhGia ADD OwnerResponseAt DATETIME2 NULL;
GO

IF COL_LENGTH(N'dbo.DanhGia', N'OwnerReportReason') IS NULL
    ALTER TABLE dbo.DanhGia ADD OwnerReportReason NVARCHAR(1000) NULL;
GO

IF COL_LENGTH(N'dbo.DanhGia', N'OwnerReportedAt') IS NULL
    ALTER TABLE dbo.DanhGia ADD OwnerReportedAt DATETIME2 NULL;
GO

IF COL_LENGTH(N'dbo.KhachSan', N'ChinhSachHuy') IS NULL
    ALTER TABLE dbo.KhachSan ADD ChinhSachHuy NVARCHAR(2000) NULL;
GO
