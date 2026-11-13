/* ============================================================
   Stayro - Đổi tên bảng tiếng Anh sang tiếng Việt
   - Chạy trên bản BACKUP trước.
   - @Rollback = 1 để đổi ngược lại tên cũ.
   - Script chạy lại nhiều lần vẫn an toàn (bỏ qua tên đã đổi).
   ============================================================ */
USE [Stayro];
GO
SET NOCOUNT ON;
DECLARE @Rollback bit = 0;   -- 0 = đổi sang tiếng Việt, 1 = hoàn tác

DECLARE @Bang TABLE (TenCu sysname, TenMoi sysname);
INSERT INTO @Bang (TenCu, TenMoi) VALUES
 (N'administrative_regions', N'VungHanhChinh'),
 (N'administrative_units',   N'DonViHanhChinh'),
 (N'provinces',              N'TinhThanh'),
 (N'wards',                  N'PhuongXa'),
 (N'ExternalLogins',         N'DangNhapNgoai'),
 (N'HotelPromotions',        N'KhuyenMaiKhachSan'),
 (N'HotelTags',              N'KhachSan_TheTag'),
 (N'Tags',                   N'TheTag'),
 (N'RefreshToken',           N'TokenLamMoi'),
 (N'Roles',                  N'VaiTro'),
 (N'UserRoles',              N'NguoiDung_VaiTro'),
 (N'RoomAvailabilityBlocks', N'KhoaPhong'),
 (N'UserLogin',              N'TaiKhoan'),
 (N'UserProfile',            N'NguoiDung'),
 (N'KhachSanImages',         N'KhachSanAnh');

DECLARE @RangBuoc TABLE (TenCu sysname, TenMoi sysname);
INSERT INTO @RangBuoc (TenCu, TenMoi) VALUES
 (N'administrative_regions_pkey',              N'PK_VungHanhChinh'),
 (N'administrative_units_pkey',                N'PK_DonViHanhChinh'),
 (N'provinces_pkey',                           N'PK_TinhThanh'),
 (N'wards_pkey',                               N'PK_PhuongXa'),
 (N'provinces_administrative_unit_id_fkey',    N'FK_TinhThanh_DonViHanhChinh'),
 (N'wards_administrative_unit_id_fkey',        N'FK_PhuongXa_DonViHanhChinh'),
 (N'wards_province_code_fkey',                 N'FK_PhuongXa_TinhThanh'),
 (N'uq_userprofile_userloginid',               N'UQ_NguoiDung_UserLoginId'),
 (N'FK_ExternalLogins_UserLogin',              N'FK_DangNhapNgoai_TaiKhoan'),
 (N'PK_HotelPromotions',                       N'PK_KhuyenMaiKhachSan'),
 (N'FK_HotelPromotions_KhachSan',              N'FK_KhuyenMaiKhachSan_KhachSan'),
 (N'CK_HotelPromotions_DiscountType',          N'CK_KhuyenMaiKhachSan_DiscountType'),
 (N'CK_HotelPromotions_DiscountValue',         N'CK_KhuyenMaiKhachSan_DiscountValue'),
 (N'CK_HotelPromotions_Range',                 N'CK_KhuyenMaiKhachSan_Range'),
 (N'PK_HotelTags',                             N'PK_KhachSan_TheTag'),
 (N'FK_HotelTags_KhachSan',                    N'FK_KhachSan_TheTag_KhachSan'),
 (N'FK_HotelTags_Tags',                        N'FK_KhachSan_TheTag_TheTag'),
 (N'PK_Tags',                                  N'PK_TheTag'),
 (N'DF_Tags_IsActive',                         N'DF_TheTag_IsActive'),
 (N'DF_Tags_SortOrder',                        N'DF_TheTag_SortOrder'),
 (N'DF_Tags_CreatedAt',                        N'DF_TheTag_CreatedAt'),
 (N'PK_RoomAvailabilityBlocks',                N'PK_KhoaPhong'),
 (N'FK_RoomAvailabilityBlocks_Phong',          N'FK_KhoaPhong_Phong'),
 (N'CK_RoomAvailabilityBlocks_Range',          N'CK_KhoaPhong_Range');

-- Đảo chiều nếu hoàn tác
IF @Rollback = 1
BEGIN
    UPDATE @Bang      SET TenCu = TenMoi, TenMoi = TenCu;
    UPDATE @RangBuoc  SET TenCu = TenMoi, TenMoi = TenCu;
END

BEGIN TRY
    BEGIN TRAN;

    DECLARE @cu sysname, @moi sysname, @full nvarchar(300);

    -- 1) Đổi tên bảng
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT TenCu, TenMoi FROM @Bang;
    OPEN cur; FETCH NEXT FROM cur INTO @cu, @moi;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @full = N'dbo.' + QUOTENAME(@cu);
        IF OBJECT_ID(@full, N'U') IS NOT NULL AND OBJECT_ID(N'dbo.' + QUOTENAME(@moi), N'U') IS NULL
        BEGIN
            EXEC sp_rename @full, @moi;
            PRINT N'Bảng: ' + @cu + N' -> ' + @moi;
        END
        ELSE PRINT N'Bỏ qua bảng: ' + @cu;
        FETCH NEXT FROM cur INTO @cu, @moi;
    END
    CLOSE cur; DEALLOCATE cur;

    -- 2) Đổi tên PK / FK / CK / DF / UQ cho đồng bộ
    DECLARE cur2 CURSOR LOCAL FAST_FORWARD FOR SELECT TenCu, TenMoi FROM @RangBuoc;
    OPEN cur2; FETCH NEXT FROM cur2 INTO @cu, @moi;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @full = N'dbo.' + QUOTENAME(@cu);
        IF OBJECT_ID(@full) IS NOT NULL AND OBJECT_ID(N'dbo.' + QUOTENAME(@moi)) IS NULL
        BEGIN
            EXEC sp_rename @full, @moi, N'OBJECT';
            PRINT N'Ràng buộc: ' + @cu + N' -> ' + @moi;
        END
        ELSE PRINT N'Bỏ qua ràng buộc: ' + @cu;
        FETCH NEXT FROM cur2 INTO @cu, @moi;
    END
    CLOSE cur2; DEALLOCATE cur2;

    COMMIT TRAN;
    PRINT N'Hoàn tất.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    PRINT N'LỖI: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

/* ------------------------------------------------------------
   Kiểm tra: còn view / stored proc / function nào tham chiếu tên cũ?
   (sp_rename KHÔNG tự cập nhật nội dung các object này)
   Chạy TRƯỚC khi đổi tên để biết cần sửa gì.
   ------------------------------------------------------------
SELECT DISTINCT OBJECT_NAME(referencing_id) AS DoiTuongThamChieu,
       referenced_entity_name AS TenBang
FROM sys.sql_expression_dependencies
WHERE referenced_entity_name IN
 (N'administrative_regions',N'administrative_units',N'provinces',N'wards',
  N'ExternalLogins',N'HotelPromotions',N'HotelTags',N'Tags',N'RefreshToken',
  N'Roles',N'UserRoles',N'RoomAvailabilityBlocks',N'UserLogin',N'UserProfile',N'KhachSanImages');
*/