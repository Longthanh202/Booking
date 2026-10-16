/*
    Role-only authorization migration.

    Back up the database and confirm no external application depends on these
    authorization tables before running this script. Roles and UserRoles are
    intentionally retained because login and [Authorize(Roles = "...")] use them.
*/
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.RolePermissions', N'U') IS NOT NULL
        DROP TABLE [dbo].[RolePermissions];

    IF OBJECT_ID(N'dbo.Permissions', N'U') IS NOT NULL
        DROP TABLE [dbo].[Permissions];

    IF OBJECT_ID(N'dbo.Resources', N'U') IS NOT NULL
        DROP TABLE [dbo].[Resources];

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO
