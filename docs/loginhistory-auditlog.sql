CREATE TABLE dbo.LoginHistory (
    Id            bigint IDENTITY(1,1) PRIMARY KEY,
    UserLoginId   uniqueidentifier NULL,         -- NULL nếu username không tồn tại
    UsernameTried nvarchar(100) NULL,
    LoginMethod   nvarchar(20)  NOT NULL,        -- PASSWORD / GOOGLE / FACEBOOK / REFRESH
    IsSuccess     bit NOT NULL,
    FailReason    nvarchar(100) NULL,            -- WRONG_PASSWORD, LOCKED, NOT_APPROVED...
    IpAddress     nvarchar(45) NULL,             -- đủ cho IPv6
    UserAgent     nvarchar(500) NULL,
    DeviceType    nvarchar(30) NULL,
    Country       nvarchar(50) NULL,
    CreatedAt     datetime2(7) NOT NULL DEFAULT sysutcdatetime(),
);
CREATE INDEX IX_LoginHistory_User ON dbo.LoginHistory(UserLoginId, CreatedAt DESC);
CREATE INDEX IX_LoginHistory_Ip   ON dbo.LoginHistory(IpAddress, CreatedAt DESC);

CREATE TABLE dbo.AuditLog (
    Id          bigint IDENTITY(1,1) PRIMARY KEY,
    ActorId     uniqueidentifier NULL,           -- NULL = hệ thống/job
    ActorRole   nvarchar(50) NULL,
    Action      nvarchar(50) NOT NULL,           -- CREATE / UPDATE / DELETE / APPROVE / REFUND...
    EntityName  nvarchar(100) NOT NULL,          -- 'DatPhong', 'GiaPhong'...
    EntityId    nvarchar(50)  NOT NULL,
    OldValues   nvarchar(max) NULL,              -- JSON
    NewValues   nvarchar(max) NULL,              -- JSON
    IpAddress   nvarchar(45) NULL,
    CorrelationId uniqueidentifier NULL,
    CreatedAt   datetime2(7) NOT NULL DEFAULT sysutcdatetime()
);
CREATE INDEX IX_Audit_Entity ON dbo.AuditLog(EntityName, EntityId, CreatedAt DESC);
CREATE INDEX IX_Audit_Actor  ON dbo.AuditLog(ActorId, CreatedAt DESC);