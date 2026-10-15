/*
    Du lieu mau de test cac bang nghiep vu trong database Stayro.

    Dieu kien:
    - Da chay docs/database-stayro-non-provinces.sql va da co it nhat mot KhachSan,
      mot tai khoan co role Customer, va du lieu role hien huu.
    - Script chi doc KhachSan/UserLogin/UserRoles/Roles/TaiKhoanNganHang; khong insert
      vao KhachSan, cac bang user, bang role/permission, hoac TaiKhoanNganHang.
    - Payout va chi tiet payout chi duoc tao neu Owner cua khach san da co tai khoan
      ngan hang trong database.
    - Payout dung ten cot theo DDL trong file schema (MaKhachSan,
      MaTaiKhoanNganHang, MaChiTra); cac ten nay khac ten property trong mot so
      entity EF hien tai va can duoc map dong bo neu muon test payout qua API.
    - TrangThai payout mau la CHO_CHI_TRA theo enum FinanceService; repository
      danh sach payout cho xu ly hien loc CHO_CHUYEN, nen endpoint pending co the
      khong tra ve payout mau cho den khi hai enum/logic duoc dong bo.
    - Cac du lieu co marker [TEST DATA] duoc tao idempotent; chay lai khong tao them
      cac booking, review, hoa hong, hoa don, giao dich vi hoac payout trung lap.
*/

USE [Stayro];
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE
        @Today date = CONVERT(date, GETDATE()),
        @HotelId uniqueidentifier,
        @OwnerId uniqueidentifier,
        @CustomerId uniqueidentifier,
        @BankAccountId uniqueidentifier,
        @AmenityId uniqueidentifier,
        @TagId uniqueidentifier,
        @StandardRoomTypeId uniqueidentifier,
        @DeluxeRoomTypeId uniqueidentifier,
        @StandardRoomId uniqueidentifier,
        @DeluxeRoomId uniqueidentifier,
        @PackageId int,
        @CompletedBookingId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150001',
        @PendingBookingId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150002',
        @CompletedDetailId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150011',
        @PendingDetailId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150012',
        @CompletedPaymentId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150021',
        @PendingPaymentId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150022',
        @CompletedRoomBookingId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150031',
        @PendingRoomBookingId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150032',
        @ReviewId uniqueidentifier = 'a8b9218b-f5a4-4e11-8c12-42cbe2150041',
        @PriceStart datetime = DATEADD(DAY, -30, CONVERT(date, GETDATE())),
        @PriceEnd datetime = DATEADD(DAY, 365, CONVERT(date, GETDATE())),
        @CompletedCheckIn date = DATEADD(DAY, -21, CONVERT(date, GETDATE())),
        @CompletedCheckOut date = DATEADD(DAY, -19, CONVERT(date, GETDATE())),
        @PendingCheckIn date = DATEADD(DAY, 14, CONVERT(date, GETDATE())),
        @PendingCheckOut date = DATEADD(DAY, 16, CONVERT(date, GETDATE())),
        @CommissionId bigint,
        @InvoiceId bigint,
        @WalletId bigint,
        @PayoutId bigint;

    SELECT TOP (1)
        @HotelId = h.Id,
        @OwnerId = h.NguoiTao
    FROM dbo.KhachSan AS h
    ORDER BY CASE WHEN h.TrangThai = N'DA_DUYET' THEN 0 ELSE 1 END,
             h.NgayTao DESC;

    IF @HotelId IS NULL
        THROW 51000, N'Can co it nhat mot KhachSan truoc khi chay script du lieu mau.', 1;

    SELECT TOP (1) @CustomerId = ur.UserId
    FROM dbo.UserRoles AS ur
    INNER JOIN dbo.Roles AS r ON r.Id = ur.RoleId
    WHERE r.RoleName = N'Customer'
    ORDER BY ur.UserId;

    IF @CustomerId IS NULL
    BEGIN
        SELECT TOP (1) @CustomerId = dp.KhachHangId
        FROM dbo.DatPhong AS dp
        WHERE dp.KhachHangId IS NOT NULL
        ORDER BY dp.NgayTao DESC;
    END;

    IF @CustomerId IS NULL
        THROW 51001, N'Can co tai khoan Customer hoac booking co KhachHangId de tao booking mau.', 1;

    SELECT TOP (1) @BankAccountId = bank.Id
    FROM dbo.TaiKhoanNganHang AS bank
    WHERE bank.UserId = @OwnerId
    ORDER BY bank.IsDefault DESC, bank.CreatedDate DESC;

    /*
        Banner: Id tu tang identity; IsActive=1 de hien thi tren giao dien public.
    */
    IF NOT EXISTS (
        SELECT 1 FROM dbo.Banner WHERE Title = N'[TEST DATA] Banner Stayro'
    )
    BEGIN
        INSERT dbo.Banner (Title, Subtitle, Url, IsActive, CreatedDate)
        VALUES (
            N'[TEST DATA] Banner Stayro',
            N'Banner dung cho moi truong kiem thu',
            N'https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1200&q=80',
            1,
            GETDATE()
        );
    END;

    /*
        Danh muc tien ich, lien ket tien ich-khach san, tag va lien ket tag-khach san.
    */
    SELECT TOP (1) @AmenityId = Id
    FROM dbo.TienIch
    WHERE TenTienIch = N'[TEST DATA] Wi-Fi kiem thu';

    IF @AmenityId IS NULL
    BEGIN
        SET @AmenityId = NEWID();
        INSERT dbo.TienIch (Id, TenTienIch, Icon)
        VALUES (@AmenityId, N'[TEST DATA] Wi-Fi kiem thu', N'wifi');
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.KhachSan_TienIch
        WHERE KhachSanId = @HotelId AND TienIchId = @AmenityId
    )
    BEGIN
        INSERT dbo.KhachSan_TienIch (KhachSanId, TienIchId)
        VALUES (@HotelId, @AmenityId);
    END;

    SELECT TOP (1) @TagId = Id
    FROM dbo.Tags
    WHERE Slug = N'test-stayro';

    IF @TagId IS NULL
    BEGIN
        SET @TagId = NEWID();
        INSERT dbo.Tags (Id, Name, Slug, Description, IsActive, SortOrder, CreatedAt)
        VALUES (
            @TagId,
            N'Du lieu kiem thu',
            N'test-stayro',
            N'Tag du lieu mau; co the dung de test danh sach khach san theo tag.',
            1,
            999,
            SYSUTCDATETIME()
        );
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.HotelTags
        WHERE HotelId = @HotelId AND TagId = @TagId
    )
    BEGIN
        INSERT dbo.HotelTags (HotelId, TagId)
        VALUES (@HotelId, @TagId);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.KhachSanImages
        WHERE KhachSanId = @HotelId
          AND Url = N'https://images.unsplash.com/photo-1564501049412-61c2a3083791?auto=format&fit=crop&w=1200&q=80'
    )
    BEGIN
        INSERT dbo.KhachSanImages (KhachSanId, Url)
        VALUES (
            @HotelId,
            N'https://images.unsplash.com/photo-1564501049412-61c2a3083791?auto=format&fit=crop&w=1200&q=80'
        );
    END;

    /*
        Loai phong, phong vat ly, gia va mot block bao tri o ngay tuong lai.
    */
    SELECT TOP (1) @StandardRoomTypeId = Id
    FROM dbo.LoaiPhong
    WHERE KhachSanId = @HotelId AND TenLoaiPhong = N'[TEST DATA] Standard';

    IF @StandardRoomTypeId IS NULL
    BEGIN
        SET @StandardRoomTypeId = NEWID();
        INSERT dbo.LoaiPhong (Id, KhachSanId, TenLoaiPhong, SoKhachToiDa, KieuGiuong, MoTa, NgayTao)
        VALUES (
            @StandardRoomTypeId, @HotelId, N'[TEST DATA] Standard', 2,
            N'1 giuong doi', N'Loai phong mau cho kiem thu API dat phong.', GETDATE()
        );
    END;

    SELECT TOP (1) @DeluxeRoomTypeId = Id
    FROM dbo.LoaiPhong
    WHERE KhachSanId = @HotelId AND TenLoaiPhong = N'[TEST DATA] Deluxe';

    IF @DeluxeRoomTypeId IS NULL
    BEGIN
        SET @DeluxeRoomTypeId = NEWID();
        INSERT dbo.LoaiPhong (Id, KhachSanId, TenLoaiPhong, SoKhachToiDa, KieuGiuong, MoTa, NgayTao)
        VALUES (
            @DeluxeRoomTypeId, @HotelId, N'[TEST DATA] Deluxe', 3,
            N'1 giuong king', N'Loai phong mau co suc chua ba khach.', GETDATE()
        );
    END;

    SELECT TOP (1) @StandardRoomId = Id
    FROM dbo.Phong
    WHERE LoaiPhongId = @StandardRoomTypeId AND SoPhong = N'TEST-101';

    IF @StandardRoomId IS NULL
    BEGIN
        SET @StandardRoomId = NEWID();
        INSERT dbo.Phong (Id, LoaiPhongId, SoPhong, Tang, TrangThai)
        VALUES (@StandardRoomId, @StandardRoomTypeId, N'TEST-101', 1, N'SAN_SANG');
    END;

    SELECT TOP (1) @DeluxeRoomId = Id
    FROM dbo.Phong
    WHERE LoaiPhongId = @DeluxeRoomTypeId AND SoPhong = N'TEST-201';

    IF @DeluxeRoomId IS NULL
    BEGIN
        SET @DeluxeRoomId = NEWID();
        INSERT dbo.Phong (Id, LoaiPhongId, SoPhong, Tang, TrangThai)
        VALUES (@DeluxeRoomId, @DeluxeRoomTypeId, N'TEST-201', 2, N'SAN_SANG');
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.GiaPhong WHERE LoaiPhongId = @StandardRoomTypeId)
    BEGIN
        INSERT dbo.GiaPhong (Id, LoaiPhongId, Gia, NgayBatDau, NgayKetThuc, IsActive, NgayTao)
        VALUES (NEWID(), @StandardRoomTypeId, 600000, @PriceStart, @PriceEnd, 1, GETDATE());
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.GiaPhong WHERE LoaiPhongId = @DeluxeRoomTypeId)
    BEGIN
        INSERT dbo.GiaPhong (Id, LoaiPhongId, Gia, NgayBatDau, NgayKetThuc, IsActive, NgayTao)
        VALUES (NEWID(), @DeluxeRoomTypeId, 950000, @PriceStart, @PriceEnd, 1, GETDATE());
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.RoomAvailabilityBlocks
        WHERE Id = 'a8b9218b-f5a4-4e11-8c12-42cbe2150051'
    )
    BEGIN
        INSERT dbo.RoomAvailabilityBlocks
            (Id, RoomId, StartAt, EndAt, Reason, CreatedBy, CreatedAt)
        VALUES (
            'a8b9218b-f5a4-4e11-8c12-42cbe2150051',
            @StandardRoomId,
            DATEADD(DAY, 40, CONVERT(datetime2, @Today)),
            DATEADD(DAY, 41, CONVERT(datetime2, @Today)),
            N'[TEST DATA] Bao tri phong',
            @OwnerId,
            SYSUTCDATETIME()
        );
    END;

    /*
        Promotion phan tram dang hoat dong, CreatedBy la Owner da co san.
    */
    IF NOT EXISTS (
        SELECT 1 FROM dbo.HotelPromotions
        WHERE HotelId = @HotelId AND Code = N'TESTSTAYRO10'
    )
    BEGIN
        INSERT dbo.HotelPromotions (
            Id, HotelId, Code, Name, DiscountType, DiscountValue,
            MinBookingAmount, MaxDiscountAmount, StartsAt, EndsAt,
            IsActive, CreatedBy, CreatedAt
        )
        VALUES (
            NEWID(), @HotelId, N'TESTSTAYRO10', N'[TEST DATA] Giam 10 phan tram',
            N'PERCENT', 10, 500000, 250000,
            DATEADD(DAY, -1, SYSUTCDATETIME()), DATEADD(DAY, 60, SYSUTCDATETIME()),
            1, @OwnerId, SYSUTCDATETIME()
        );
    END;

    /*
        Hai booking khong chong ngay: mot da checkout de test review/hoa hong;
        mot cho xac nhan trong tuong lai de test danh sach booking va thanh toan.
    */
    IF EXISTS (
        SELECT 1 FROM dbo.DatPhong
        WHERE Id = @CompletedBookingId AND KhachSanId <> @HotelId
    )
        THROW 51002, N'Booking ID test mau da ton tai nhung khong thuoc khach san duoc chon.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.DatPhong WHERE Id = @CompletedBookingId)
    BEGIN
        INSERT dbo.DatPhong (
            Id, KhachHangId, KhachSanId, NgayNhanPhong, NgayTraPhong,
            TongTien, TrangThai, NgayTao, SoTienGiam, MaKhuyenMai
        )
        VALUES (
            @CompletedBookingId, @CustomerId, @HotelId,
            @CompletedCheckIn, @CompletedCheckOut, 1200000,
            N'DA_CHECK_OUT', DATEADD(DAY, -22, CONVERT(datetime, @Today)), 0, NULL
        );
    END;

    IF EXISTS (
        SELECT 1 FROM dbo.DatPhong
        WHERE Id = @PendingBookingId AND KhachSanId <> @HotelId
    )
        THROW 51003, N'Booking ID test mau da ton tai nhung khong thuoc khach san duoc chon.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.DatPhong WHERE Id = @PendingBookingId)
    BEGIN
        INSERT dbo.DatPhong (
            Id, KhachHangId, KhachSanId, NgayNhanPhong, NgayTraPhong,
            TongTien, TrangThai, NgayTao, SoTienGiam, MaKhuyenMai
        )
        VALUES (
            @PendingBookingId, @CustomerId, @HotelId,
            @PendingCheckIn, @PendingCheckOut, 1900000,
            N'CHO_XAC_NHAN', GETDATE(), 0, NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.ChiTietDatPhong WHERE Id = @CompletedDetailId)
    BEGIN
        INSERT dbo.ChiTietDatPhong (Id, DatPhongId, LoaiPhongId, SoLuongPhong, GiaMoiDem)
        VALUES (@CompletedDetailId, @CompletedBookingId, @DeluxeRoomTypeId, 1, 600000);
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.ChiTietDatPhong WHERE Id = @PendingDetailId)
    BEGIN
        INSERT dbo.ChiTietDatPhong (Id, DatPhongId, LoaiPhongId, SoLuongPhong, GiaMoiDem)
        VALUES (@PendingDetailId, @PendingBookingId, @StandardRoomTypeId, 1, 950000);
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.PhongDat WHERE Id = @CompletedRoomBookingId)
    BEGIN
        INSERT dbo.PhongDat (Id, ChiTietDatPhongId, PhongId, TrangThai)
        VALUES (@CompletedRoomBookingId, @CompletedDetailId, @DeluxeRoomId, N'DA_CHECK_OUT');
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.PhongDat WHERE Id = @PendingRoomBookingId)
    BEGIN
        INSERT dbo.PhongDat (Id, ChiTietDatPhongId, PhongId, TrangThai)
        VALUES (@PendingRoomBookingId, @PendingDetailId, @StandardRoomId, N'DA_DAT');
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.ThanhToan WHERE Id = @CompletedPaymentId)
    BEGIN
        INSERT dbo.ThanhToan (Id, DatPhongId, PhuongThuc, SoTien, TrangThai, ThoiGianThanhToan)
        VALUES (
            @CompletedPaymentId, @CompletedBookingId, N'cash', 1200000,
            N'DA_THANH_TOAN', DATEADD(DAY, -19, CONVERT(datetime, @Today))
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.ThanhToan WHERE Id = @PendingPaymentId)
    BEGIN
        INSERT dbo.ThanhToan (Id, DatPhongId, PhuongThuc, SoTien, TrangThai, ThoiGianThanhToan)
        VALUES (@PendingPaymentId, @PendingBookingId, N'cash', 1900000, N'CHO_THANH_TOAN', NULL);
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.DanhGia WHERE Id = @ReviewId)
    BEGIN
        INSERT dbo.DanhGia (
            Id, KhachSanId, KhachHangId, SoSao, NoiDung, NgayTao,
            OwnerResponse, OwnerResponseAt, OwnerReportReason, OwnerReportedAt
        )
        VALUES (
            @ReviewId, @HotelId, @CustomerId, 5,
            N'[TEST DATA] Phong sach se, nhan vien than thien.',
            DATEADD(DAY, -18, CONVERT(datetime, @Today)),
            N'[TEST DATA] Cam on ban da danh gia.', SYSUTCDATETIME(), NULL, NULL
        );
    END;

    /*
        Goi quang cao va chien dich dang hien thi de test boost ket qua tim kiem.
    */
    SELECT TOP (1) @PackageId = Id
    FROM dbo.GoiQuangCao
    WHERE TenGoi = N'[TEST DATA] Goi uu tien 7 ngay';

    IF @PackageId IS NULL
    BEGIN
        INSERT dbo.GoiQuangCao (TenGoi, SoNgay, GiaTien, DiemUuTien, IsActive, CreatedDate)
        VALUES (N'[TEST DATA] Goi uu tien 7 ngay', 7, 500000, 10, 1, GETDATE());
        SET @PackageId = CONVERT(int, SCOPE_IDENTITY());
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.KhachSanQuangCao
        WHERE KhachSanId = @HotelId
          AND GoiQuanCaoId = @PackageId
          AND TrangThai = N'DANG_HIEN_THI'
    )
    BEGIN
        INSERT dbo.KhachSanQuangCao (
            KhachSanId, GoiQuanCaoId, NgayBatDau, NgayKetThuc,
            DiemUuTien, TrangThai, CreatedDate
        )
        VALUES (
            @HotelId, @PackageId, GETDATE(), DATEADD(DAY, 7, GETDATE()),
            10, N'DANG_HIEN_THI', GETDATE()
        );
    END;

    /*
        Hoa hong va hoa don cho booking da checkout.
    */
    SELECT TOP (1) @CommissionId = MaHoaHong
    FROM dbo.HoaHong
    WHERE MaDatPhong = @CompletedBookingId AND MaKhachSan = @HotelId;

    IF @CommissionId IS NULL
    BEGIN
        INSERT dbo.HoaHong (
            MaDatPhong, MaKhachSan, TyLeHoaHong, SoTienHoaHong,
            TrangThai, NgayTao, NgayThu
        )
        VALUES (
            @CompletedBookingId, @HotelId, 15, 180000,
            N'CHO_THU', DATEADD(DAY, -19, CONVERT(datetime, @Today)), NULL
        );
        SET @CommissionId = CONVERT(bigint, SCOPE_IDENTITY());
    END;

    DECLARE
        @InvoiceFrom datetime2(7) = DATEFROMPARTS(YEAR(@CompletedCheckOut), MONTH(@CompletedCheckOut), 1),
        @InvoiceTo datetime2(7) = DATEADD(MONTH, 1, DATEFROMPARTS(YEAR(@CompletedCheckOut), MONTH(@CompletedCheckOut), 1));

    SELECT TOP (1) @InvoiceId = Id
    FROM dbo.HoaDonHoaHong
    WHERE KhachSanId = @HotelId
      AND TuNgay = @InvoiceFrom
      AND DenNgay = @InvoiceTo;

    IF @InvoiceId IS NULL
    BEGIN
        INSERT dbo.HoaDonHoaHong (
            KhachSanId, TuNgay, DenNgay, TongTienHoaHong, TrangThai, NgayTao
        )
        VALUES (@HotelId, @InvoiceFrom, @InvoiceTo, 180000, N'CHO_THANH_TOAN', SYSUTCDATETIME());
        SET @InvoiceId = CONVERT(bigint, SCOPE_IDENTITY());
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.ChiTietHoaDonHoaHong
        WHERE HoaDonHoaHongId = @InvoiceId AND HoaHongId = @CommissionId
    )
    BEGIN
        INSERT dbo.ChiTietHoaDonHoaHong (HoaDonHoaHongId, HoaHongId)
        VALUES (@InvoiceId, @CommissionId);
    END;

    /*
        Vi va lich su: ghi nhan tien phong 1.200.000, tru hoa hong 180.000.
        Neu co tai khoan ngan hang cua Owner, tao them mot payout cho so tien thuc
        nhan 1.020.000, dong bo SoDu/SoDuTamGiu va chi tiet payout.
    */
    SELECT TOP (1) @WalletId = MaVi
    FROM dbo.ViKhachSan
    WHERE MaKhachSan = @HotelId;

    IF @WalletId IS NULL
    BEGIN
        INSERT dbo.ViKhachSan (MaKhachSan, SoDu, SoDuTamGiu, NgayCapNhat, TrangThai)
        VALUES (@HotelId, 0, 0, GETDATE(), N'HOAT_DONG');
        SET @WalletId = CONVERT(bigint, SCOPE_IDENTITY());
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.LichSuVi
        WHERE MaVi = @WalletId AND NoiDung = N'[TEST DATA] Cong tien booking da checkout'
    )
    BEGIN
        UPDATE dbo.ViKhachSan SET SoDu = SoDu + 1200000, NgayCapNhat = GETDATE()
        WHERE MaVi = @WalletId;
        INSERT dbo.LichSuVi (MaVi, MaDatPhong, SoTien, LoaiGiaoDich, NoiDung, NgayTao)
        VALUES (@WalletId, @CompletedBookingId, 1200000, N'CONG_TIEN',
                N'[TEST DATA] Cong tien booking da checkout', GETDATE());
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.LichSuVi
        WHERE MaVi = @WalletId AND NoiDung = N'[TEST DATA] Tru hoa hong booking'
    )
    BEGIN
        UPDATE dbo.ViKhachSan SET SoDu = SoDu - 180000, NgayCapNhat = GETDATE()
        WHERE MaVi = @WalletId;
        INSERT dbo.LichSuVi (MaVi, MaDatPhong, SoTien, LoaiGiaoDich, NoiDung, NgayTao)
        VALUES (@WalletId, @CompletedBookingId, 180000, N'TRU_HOA_HONG',
                N'[TEST DATA] Tru hoa hong booking', GETDATE());
    END;

    IF @BankAccountId IS NOT NULL
    BEGIN
        SELECT TOP (1) @PayoutId = MaChiTra
        FROM dbo.ChiTraKhachSan
        WHERE MaKhachSan = @HotelId
          AND GhiChu = N'[TEST DATA] Rut tien tu booking mau';

        IF @PayoutId IS NULL
        BEGIN
            UPDATE dbo.ViKhachSan
            SET SoDu = SoDu - 1020000,
                SoDuTamGiu = SoDuTamGiu + 1020000,
                NgayCapNhat = GETDATE()
            WHERE MaVi = @WalletId AND SoDu >= 1020000;

            IF @@ROWCOUNT = 0
                THROW 51004, N'So du vi khong du de tao payout mau; kiem tra so du vi truoc khi chay lai.', 1;

            INSERT dbo.ChiTraKhachSan (
                MaKhachSan, MaTaiKhoanNganHang, SoTien, TrangThai,
                MaGiaoDich, GhiChu, NgayTao, NgayChuyenTien
            )
            VALUES (
                @HotelId, @BankAccountId, 1020000, N'CHO_CHI_TRA',
                NULL, N'[TEST DATA] Rut tien tu booking mau', GETDATE(), NULL
            );
            SET @PayoutId = CONVERT(bigint, SCOPE_IDENTITY());

            INSERT dbo.LichSuVi (MaVi, MaDatPhong, SoTien, LoaiGiaoDich, NoiDung, NgayTao)
            VALUES (@WalletId, @CompletedBookingId, 1020000, N'RUT_TIEN',
                    N'[TEST DATA] Tao yeu cau rut tien', GETDATE());
        END;

        IF NOT EXISTS (
            SELECT 1 FROM dbo.ChiTietChiTraKhachSan
            WHERE MaChiTra = @PayoutId AND MaDatPhong = @CompletedBookingId
        )
        BEGIN
            INSERT dbo.ChiTietChiTraKhachSan (
                MaChiTra, MaDatPhong, SoTienThanhToan, SoTienHoaHong, SoTienThucNhan
            )
            VALUES (@PayoutId, @CompletedBookingId, 1200000, 180000, 1020000);
        END;
    END;

    COMMIT TRANSACTION;
    PRINT N'Da tao du lieu test. KhachSanId = ' + CONVERT(nvarchar(36), @HotelId);
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
