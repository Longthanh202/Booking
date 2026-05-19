CREATE INDEX IX_KhachSan_Search
ON KhachSan (ThanhPho)
INCLUDE (TenKhachSan, SoSao, DiaChi, NgayTao)

CREATE INDEX IX_LoaiPhong_KhachSan
ON LoaiPhong (KhachSanId, SoKhachToiDa)

CREATE INDEX IX_Phong_LoaiPhong
ON Phong (LoaiPhongId)

CREATE INDEX IX_QC_Active
ON KhachSanQuangCao (
    KhachSanId,
    TrangThai,
    NgayBatDau,
    NgayKetThuc
)
INCLUDE (DiemUuTien)

