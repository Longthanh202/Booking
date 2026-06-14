create type resources_permissions as table
(
	ResourceId uniqueidentifier,
	PermissionId uniqueidentifier
)

create type hotel_images as table
(
	Url nvarchar(1000)
)

create type tienich as table
(
	tienIchId uniqueidentifier
)

CREATE TYPE HotelIdList AS TABLE
(
    Id UNIQUEIDENTIFIER
)

CREATE TYPE TVP_ChiTietDatPhong AS TABLE
(
    LoaiPhongId UNIQUEIDENTIFIER,
    SoLuongPhong INT,
    GiaMoiDem DECIMAL(18,2)
);
GO