IF OBJECT_ID(N'dbo.Tags', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tags
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Tags PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        Slug NVARCHAR(120) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Tags_IsActive DEFAULT (1),
        SortOrder INT NOT NULL CONSTRAINT DF_Tags_SortOrder DEFAULT (0),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Tags_CreatedAt DEFAULT (SYSUTCDATETIME())
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Tags_Slug'
      AND object_id = OBJECT_ID(N'dbo.Tags')
)
BEGIN
    CREATE UNIQUE INDEX UX_Tags_Slug ON dbo.Tags(Slug);
END;
GO

IF OBJECT_ID(N'dbo.HotelTags', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HotelTags
    (
        HotelId UNIQUEIDENTIFIER NOT NULL,
        TagId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_HotelTags PRIMARY KEY (HotelId, TagId),
        CONSTRAINT FK_HotelTags_KhachSan FOREIGN KEY (HotelId)
            REFERENCES dbo.KhachSan(Id) ON DELETE CASCADE,
        CONSTRAINT FK_HotelTags_Tags FOREIGN KEY (TagId)
            REFERENCES dbo.Tags(Id) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_HotelTags_TagId_HotelId'
      AND object_id = OBJECT_ID(N'dbo.HotelTags')
)
BEGIN
    CREATE INDEX IX_HotelTags_TagId_HotelId ON dbo.HotelTags(TagId, HotelId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tags WHERE Slug = N'da-lat')
BEGIN
    INSERT INTO dbo.Tags (Name, Slug, Description, IsActive, SortOrder)
    VALUES (N'Đà Lạt', N'da-lat', N'Khách sạn tại Đà Lạt', 1, 1);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tags WHERE Slug = N'sapa')
BEGIN
    INSERT INTO dbo.Tags (Name, Slug, Description, IsActive, SortOrder)
    VALUES (N'Sapa', N'sapa', N'Khách sạn tại Sapa', 1, 2);
END;
GO
