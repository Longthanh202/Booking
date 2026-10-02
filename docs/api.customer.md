# Customer API

## Cấu hình

- Base URL mặc định: `http://localhost:5295`
- Route yêu cầu đăng nhập: `Authorization: Bearer {{accessToken}}`
- JSON: `Content-Type: application/json`; ID là UUID; ngày giờ dùng ISO 8601.

## Tài khoản và khám phá

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `POST` | `/api/users/register` | Public | Đăng ký |
| `POST` | `/api/users/login` | Public | Đăng nhập |
| `POST` | `/api/auth/google-login` | Public | Đăng nhập Google bằng `idToken` |
| `GET` | `/api/users/me` | Đã đăng nhập | Hồ sơ và quyền hiện tại |
| `POST` | `/api/users/refresh-token` | Public | Làm mới token từ refresh-token cookie |
| `POST` | `/api/users/forgot-password` | Public | Gửi yêu cầu reset mật khẩu |
| `POST` | `/api/users/reset-password` | Public | Xác nhận reset mật khẩu |
| `GET` | `/api/provinces` | Public | Tỉnh/thành chuẩn |
| `GET` | `/api/tags` | Public | Tab/tag đang hoạt động |
| `GET` | `/api/tags/{slug}/hotels?page=1&pageSize=10` | Public | Khách sạn đã duyệt theo tag |
| `GET` | `/api/hotels/search` | Public | Tìm khách sạn theo keyword, số khách, ngày và phân trang |
| `GET` | `/api/hotels/{hotelId}` | Public | Chi tiết, tiện ích, ảnh, chính sách và giờ nhận/trả |
| `POST` | `/api/room-types/by-hotel` | Public | Loại phòng và availability theo khách sạn |
| `GET` | `/api/reviews/hotel/{hotelId}` | Public | Review, gồm phản hồi Owner nếu có |
| `GET` | `/api/banners/active` | Public | Banner đang hoạt động |

Ví dụ bấm tab Đà Lạt: `GET /api/tags/da-lat/hotels?page=1&pageSize=12`. Tab chỉ đếm và trả khách sạn `DA_DUYET`.

## Booking

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `POST` | `/api/bookings` | Customer | Tạo booking, tùy chọn áp mã khuyến mãi |
| `GET` | `/api/bookings/history?pageIndex=1&pageSize=10` | Customer | Lịch sử booking |
| `GET` | `/api/bookings/{bookingId}` | Customer | Chi tiết booking của mình |
| `PUT` | `/api/bookings/{bookingId}/cancel` | Customer | Hủy booking đang chờ hoặc đã xác nhận |

Body tạo booking:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "ngayNhanPhong": "2026-10-10T14:00:00Z",
  "ngayTraPhong": "2026-10-12T12:00:00Z",
  "phuongThucThanhToan": "cash",
  "maKhuyenMai": "DALAT-10",
  "danhSachPhong": [
    {
      "loaiPhongId": "00000000-0000-0000-0000-000000000002",
      "soLuong": 1
    }
  ]
}
```

`maKhuyenMai` có thể bỏ trống. API kiểm tra mã còn hiệu lực và điều kiện đơn hàng; mã không hợp lệ trả `400`. Tổng payment là số tiền sau giảm; booking lưu `SoTienGiam` và mã đã dùng.

## Thanh toán và đánh giá

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `GET` | `/api/payments/{paymentId}` | Customer | Payment của mình |
| `GET` | `/api/payments/booking/{bookingId}/qr` | Customer | QR thanh toán booking |
| `POST` | `/api/reviews` | Customer | Đánh giá sau checkout |
| `GET` | `/api/reviews/mine` | Customer | Review của mình |

## Realtime

SignalR hub: `/bookingHub`. Client đăng nhập có thể nhận `BookingSuccess` và `BookingStatusChanged`. Thông báo hiện realtime, chưa được lưu thành inbox có API đọc lịch sử.

API kiểm tra owner của booking/payment trên backend. Lỗi phổ biến: `400` dữ liệu/trạng thái không hợp lệ, `401` token thiếu/sai, `403` không đủ quyền, `404` không tìm thấy tài nguyên.
