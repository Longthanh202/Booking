# Booking API - Postman Reference

## Cấu hình chung

- Base URL: `http://localhost:5295`
- Header JSON: `Content-Type: application/json`
- Header xác thực cho route có quyền: `Authorization: Bearer {{token}}`
- Thay các giá trị `{{...}}` bằng dữ liệu thật trong môi trường Postman.
- Các giá trị ngày dùng ISO 8601, ví dụ `2026-09-25T14:00:00Z`.

## AuthController

### POST `/api/auth/google-login`

Quyền: công khai. Body JSON:

```json
{
  "idToken": "{{google_id_token}}"
}
```

## BannerController

### POST `/api/banners`

Quyền: `Admin`. Body: `form-data`.

| Key | Type | Value mẫu |
|---|---|---|
| `Title` | Text | `Khuyến mãi mùa hè` |
| `Subtitle` | Text | `Giảm 20% khi đặt phòng` |
| `IsActive` | Text | `1` |
| `File` | File | chọn file ảnh |

### GET `/api/banners/active`

Quyền: công khai. Không có tham số.

## DatPhongController

### POST `/api/bookings`

Quyền: `Customer`. Body JSON:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "ngayNhanPhong": "2026-09-25T14:00:00Z",
  "ngayTraPhong": "2026-09-27T12:00:00Z",
  "phuongThucThanhToan": "cash",
  "danhSachPhong": [
    {
      "loaiPhongId": "00000000-0000-0000-0000-000000000002",
      "soLuong": 1
    }
  ]
}
```

### PUT `/api/bookings/{{bookingId}}/check-out`

Quyền: `Owner`. Không có body.

### POST `/api/bookings/owner`

Quyền: `Owner`. Body JSON:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/bookings/history?pageIndex=1&pageSize=10`

Quyền: `Customer`. Không có body.

### PUT `/api/bookings/{{bookingId}}/check-in`

Quyền: `Owner`. Không có body.

### PUT `/api/bookings/{{bookingId}}/confirm`

Quyền: `Owner`. Không có body.

### POST `/api/bookings/owner/statistics`

Quyền: `Owner`. Body JSON:

```json
{
  "hotelId": "00000000-0000-0000-0000-000000000001",
  "startDate": "2026-09-01T00:00:00Z",
  "endDate": "2026-09-30T23:59:59Z",
  "status": "confirmed"
}
```

### POST `/api/bookings/owner/all`

Quyền: `Owner`. Body JSON:

```json
{
  "page": 1,
  "pageSize": 10
}
```

## HoaHongController

### GET `/api/commissions/owner`

Quyền: `Owner`. Không có tham số hoặc body.

## HomeController

### GET `/api/home/health`

Quyền: công khai. Không có tham số.

## KhachSanController

### POST `/api/hotels`

Quyền: `Owner`. Body: `form-data`.

| Key | Type | Value mẫu |
|---|---|---|
| `NguoiTao` | Text | `00000000-0000-0000-0000-000000000001` |
| `TenKhachSan` | Text | `Stayro Hotel` |
| `MoTa` | Text | `Khách sạn gần trung tâm` |
| `DiaChi` | Text | `01 Đường Mẫu, Quy Nhơn` |
| `ThanhPho` | Text | `Quy Nhơn` |
| `ViDo` | Text | `13.782` |
| `KinhDo` | Text | `109.219` |
| `SoSao` | Text | `3` |
| `GioNhanPhong` | Text | `14:00` |
| `GioTraPhong` | Text | `12:00` |
| `Files` | File | chọn một hoặc nhiều ảnh |

### GET `/api/hotels/admin/search?keyword=hotel&thanhPho=Quy%20Nhon&soSao=3&trangThai=active&page=1&pageSize=10`

Quyền: `Admin`. Tất cả query đều tùy chọn:

- `keyword: string`
- `thanhPho: string`
- `viDo: number`
- `kinhDo: number`
- `soSao: int`
- `trangThai: string`
- `page: int`, mặc định `1`
- `pageSize: int`, mặc định `10`

### GET `/api/hotels/owner/search?page=1&pageSize=10`

Quyền: `Owner`. Query:

- `page: int`, mặc định `1`
- `pageSize: int`, mặc định `10`

### GET `/api/hotels/{{hotelId}}`

Quyền: công khai. `hotelId` phải là `Guid`.

### GET `/api/hotels/search?keyword=biển&soKhach=2&ngayNhanPhong=2026-09-25&ngayTraPhong=2026-09-27&page=1&pageSize=10`

Quyền: công khai. Query:

- `keyword: string`
- `soKhach: int`
- `ngayNhanPhong: DateTime`
- `ngayTraPhong: DateTime`
- `page: int`, mặc định `1`
- `pageSize: int`, mặc định `10`

## LichSuViController

### GET `/api/wallet/owner`

Quyền: `Owner`. Không có tham số hoặc body.

## LoaiPhongController

### POST `/api/room-types`

Quyền: `Owner`. Body JSON là một mảng:

```json
[
  {
    "khachSanId": "00000000-0000-0000-0000-000000000001",
    "tenLoaiPhong": "Phòng Deluxe",
    "soKhachToiDa": 2,
    "kieuGiuong": "Giường đôi",
    "moTa": "Phòng có ban công"
  }
]
```

### POST `/api/room-types/by-hotel`

Quyền: công khai. Body JSON:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "soKhach": 2,
  "ngayNhan": "2026-09-25T14:00:00Z",
  "ngayTra": "2026-09-27T12:00:00Z"
}
```

### GET `/api/room-types/owner?khachSanId={{hotelId}}`

Quyền: `Owner`. `khachSanId` là query parameter, không gửi body:

```text
GET /api/room-types/owner?khachSanId=00000000-0000-0000-0000-000000000001
```

## PhongController

### POST `/api/rooms`

Quyền: `Owner`. Body JSON:

```json
{
  "loaiPhongId": "00000000-0000-0000-0000-000000000002",
  "soPhong": "101",
  "tang": 1,
  "trangThai": "san_sang"
}
```

## ProvinceController

### GET `/api/provinces`

Quyền: công khai. Không có tham số.

## TienIchController

### POST `/api/amenities`

Quyền: `Owner`. Body JSON là một mảng:

```json
[
  {
    "tenTienIch": "WiFi 24/7",
    "icon": "wifi",
    "khachSanId": "00000000-0000-0000-0000-000000000001"
  }
]
```

## UserController

### POST `/api/users/login`

Quyền: công khai. Body JSON:

```json
{
  "username": "customer01",
  "password": "your-password"
}
```

### GET `/api/users/me`

Quyền: đăng nhập. Không có body.

### POST `/api/users/refresh-token`

Quyền: công khai theo controller. Gửi refresh token theo cơ chế mà `RefreshTokenService` đang cấu hình. Không có body.

### POST `/api/users/role-permissions`

Quyền: `Admin`. Body JSON:

```json
{
  "roleId": "00000000-0000-0000-0000-000000000003",
  "permissions": [
    {
      "resourceId": "00000000-0000-0000-0000-000000000004",
      "permissionId": "00000000-0000-0000-0000-000000000005"
    }
  ]
}
```

### POST `/api/users/register`

Quyền: công khai. Body JSON:

```json
{
  "username": "customer01",
  "password": "your-password",
  "fullName": "Nguyen Van A",
  "phone": "0900000000",
  "address": "Quy Nhon",
  "email": "customer01@example.com",
  "createBy": null
}
```

### POST `/api/users/forgot-password`

Quyền: công khai. Body JSON:

```json
{
  "username": "customer01"
}
```

## Lưu ý khi test

1. Với route yêu cầu quyền, lấy token từ `POST /api/users/login` rồi gán vào biến môi trường `token`.
2. Các route `GET` tìm kiếm khách sạn dùng query string, không dùng raw JSON body.
3. `POST /api/hotels` và `POST /api/banners` phải dùng `form-data` vì có upload file.
4. `GET /api/room-types/owner` dùng query `khachSanId`, vì HTTP GET không nên nhận request body.
5. Các tên route cũ như `/api/login`, `/api/me`, `/api/khachsan/tao`, `/api/khachsans/filter` không còn khớp với controller hiện tại.

## API bổ sung - Booking

### GET `/api/bookings/{{bookingId}}`

Quyền: `Customer`. Customer chỉ xem được booking của chính mình.

### PUT `/api/bookings/{{bookingId}}/cancel`

Quyền: `Customer`. Chỉ có thể hủy booking ở trạng thái `CHO_XAC_NHAN` hoặc `DA_XAC_NHAN`. Không có body.

### POST `/api/bookings/admin/search`

Quyền: `Admin`. Body JSON, các trường lọc có thể bỏ qua:

```json
{
  "hotelId": "00000000-0000-0000-0000-000000000001",
  "customerId": "00000000-0000-0000-0000-000000000002",
  "trangThai": "DA_XAC_NHAN",
  "ngayTao": "2026-09-23T00:00:00Z",
  "page": 1,
  "pageSize": 10
}
```

## API bổ sung - Room và Property

### GET `/api/rooms/by-room-type/{{roomTypeId}}`

Quyền: `Owner`. Trả về các phòng thuộc loại phòng của Owner đang đăng nhập.

### PUT `/api/rooms/{{roomId}}`

Quyền: `Owner`. Body JSON:

```json
{
  "soPhong": "101",
  "tang": 1,
  "trangThai": "SAN_SANG"
}
```

### DELETE `/api/rooms/{{roomId}}`

Quyền: `Owner`. Không thể xóa phòng đã phát sinh booking.

### GET `/api/rooms/prices/{{roomTypeId}}`

Quyền: `Owner`. Xem lịch giá của loại phòng.

### POST `/api/rooms/prices`

Quyền: `Owner`. Body JSON:

```json
{
  "loaiPhongId": "00000000-0000-0000-0000-000000000002",
  "gia": 850000,
  "ngayBatDau": "2026-10-01T00:00:00Z",
  "ngayKetThuc": "2026-10-31T23:59:59Z"
}
```

### PUT `/api/hotels/admin/{{hotelId}}/status`

Quyền: `Admin`. Body JSON:

```json
{
  "trangThai": "DA_DUYET"
}
```

Giá trị trạng thái: `CHO_DUYET`, `DA_DUYET`, `TAM_NGUNG`, `TU_CHOI`, `DA_KHOA`.

## API bổ sung - Review

### GET `/api/reviews/hotel/{{hotelId}}`

Quyền: công khai. Xem review của một property.

### GET `/api/reviews/mine`

Quyền: `Customer`. Xem các review của Customer hiện tại.

### POST `/api/reviews`

Quyền: `Customer`. Chỉ tạo được review sau khi đã checkout booking. Body JSON:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "soSao": 5,
  "noiDung": "Phòng sạch và nhân viên thân thiện"
}
```

### GET `/api/reviews/owner/hotel/{{hotelId}}`

Quyền: `Owner`. Owner chỉ xem review của property thuộc mình.

### DELETE `/api/reviews/{{reviewId}}`

Quyền: `Admin`. Xóa review vi phạm.

## API bổ sung - Payment

### GET `/api/payments/{{paymentId}}`

Quyền: `Customer`. Customer chỉ xem payment thuộc booking của mình.

### GET `/api/payments/admin`

Quyền: `Admin`. Xem toàn bộ payment.

### PUT `/api/payments/admin/{{paymentId}}/status`

Quyền: `Admin`. Body JSON:

```json
{
  "trangThai": "DA_THANH_TOAN"
}
```

Giá trị trạng thái: `CHO_THANH_TOAN`, `DA_THANH_TOAN`, `DA_HUY`, `DA_HOAN_TIEN`.

## API bổ sung - Hoa hồng

### GET `/api/commissions/admin/pending`

Quyền: `Admin`. Xem các khoản hoa hồng đang ở trạng thái `CHO_THU`.

## API bổ sung - Ví, nhận tiền và rút tiền

### GET `/api/finance/wallets`

Quyền: `Owner`. Xem ví của các property thuộc Owner hiện tại.

### GET `/api/finance/statement?khachSanId={{hotelId}}&from=2026-09-01&to=2026-09-30`

Quyền: `Owner`. Các query `khachSanId`, `from`, `to` đều tùy chọn. Đây là sao kê các giao dịch ví.

### GET `/api/finance/bank-accounts`

Quyền: `Owner`. Xem tài khoản ngân hàng của Owner hiện tại.

### POST `/api/finance/bank-accounts`

Quyền: `Owner`. Body JSON:

```json
{
  "tenNganHang": "Vietcombank",
  "soTaiKhoan": "0123456789",
  "chuTaiKhoan": "NGUYEN VAN A",
  "chiNhanh": "Quy Nhon",
  "qrCode": null,
  "isDefault": true
}
```

### POST `/api/finance/withdrawals`

Quyền: `Owner`. Số tiền được chuyển sang số dư tạm giữ khi tạo yêu cầu. Body JSON:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "taiKhoanNganHangId": "00000000-0000-0000-0000-000000000003",
  "soTien": 1500000,
  "ghiChu": "Rút doanh thu tháng 9"
}
```

### GET `/api/finance/withdrawals`

Quyền: `Owner`. Xem lịch sử yêu cầu rút tiền của Owner.

### GET `/api/finance/admin/withdrawals/pending`

Quyền: `Admin`. Xem các yêu cầu đang chờ chuyển tiền.

### PUT `/api/finance/admin/withdrawals/{{withdrawalId}}`

Quyền: `Admin`. Body JSON:

```json
{
  "trangThai": "DA_CHI_TRA",
  "maGiaoDich": "BANK-TXN-20260923-001",
  "ghiChu": "Đã chuyển khoản thành công"
}
```

Giá trị trạng thái: `CHO_CHI_TRA`, `DANG_XU_LY`, `DA_CHI_TRA`, `THAT_BAI`, `DA_HUY`.

## API bổ sung - Quảng cáo

### GET `/api/advertising/packages`

Quyền: công khai. Xem các gói quảng cáo đang hoạt động.

### GET `/api/advertising/owner`

Quyền: `Owner`. Xem các đơn/chiến dịch quảng cáo của Owner.

### POST `/api/advertising/orders`

Quyền: `Owner`. Tạo đơn quảng cáo, ban đầu ở trạng thái `CHO_THANH_TOAN`. Body JSON:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "goiQuangCaoId": 1
}
```

### PUT `/api/advertising/orders/{{advertisingId}}/confirm-payment`

Quyền: `Owner`. Xác nhận đã thanh toán quảng cáo. Không có body. Đơn chuyển sang `CHO_DUYET`.

### GET `/api/advertising/admin/pending`

Quyền: `Admin`. Xem các chiến dịch đã xác nhận thanh toán và chờ duyệt.

### PUT `/api/advertising/admin/{{advertisingId}}/approve`

Quyền: `Admin`. Body JSON:

```json
{
  "approved": true,
  "lyDo": null
}
```

Đặt `approved: false` để từ chối. Khi duyệt, trạng thái chuyển thành `DANG_HIEN_THI` và hệ thống tự tính `NgayKetThuc` theo số ngày của gói.

## API bổ sung - Banner Admin

### GET `/api/banners?keyword=summer&isActive=1&startRow=0&endRow=50`

Quyền: `Admin`. Xem danh sách banner, có thể lọc theo từ khóa và trạng thái.

### PUT `/api/banners/{{bannerId}}`

Quyền: `Admin`. Body JSON:

```json
{
  "title": "Khuyến mãi mùa hè",
  "subtitle": "Giảm 20% khi đặt phòng",
  "url": "https://cdn.example.com/banner.jpg",
  "isActive": 1
}
```

### DELETE `/api/banners/{{bannerId}}`

Quyền: `Admin`. Xóa banner. Không có body.

## API bổ sung - Reset mật khẩu

### POST `/api/users/reset-password`

Quyền: công khai. Gửi mã OTP nhận được từ `forgot-password` cùng mật khẩu mới:

```json
{
  "username": "customer01",
  "code": "123456",
  "newPassword": "new-password-2026"
}
```
