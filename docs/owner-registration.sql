IF COL_LENGTH('dbo.UserProfile', 'OwnerApprovalStatus') IS NULL
BEGIN
    ALTER TABLE dbo.UserProfile
        ADD OwnerApprovalStatus int NULL;
END;
GO

IF COL_LENGTH('dbo.UserProfile', 'OwnerReviewNote') IS NULL
BEGIN
    ALTER TABLE dbo.UserProfile
        ADD OwnerReviewNote nvarchar(500) NULL;
END;
GO

IF COL_LENGTH('dbo.UserProfile', 'OwnerReviewedAt') IS NULL
BEGIN
    ALTER TABLE dbo.UserProfile
        ADD OwnerReviewedAt datetime2 NULL;
END;
GO

IF COL_LENGTH('dbo.UserProfile', 'OwnerReviewedBy') IS NULL
BEGIN
    ALTER TABLE dbo.UserProfile
        ADD OwnerReviewedBy uniqueidentifier NULL;
END;
GO
