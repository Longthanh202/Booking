# Customer API

API cho khách hàng đặt khách sạn. Các route không ghi quyền là public; route Customer yêu cầu JWT.

## Cấu hình

- Base URL mặc định: `http://localhost:5295`
- Header xác thực: `Authorization: Bearer {{accessToken}}`
- JSON: `Content-Type: application/json`
- ID dùng kiểu UUID; ngày giờ gửi theo ISO 8601.

## Tài khoản

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `POST` | `/api/users/register` | Public | Đăng ký tài khoản |
| `POST` | `/api/users/login` | Public | Đăng nhập, nhận token |
| `POST` | `/api/auth/google-login` | Public | Đăng nhập Google bằng `idToken` |
| `GET` | `/api/users/me` | Đã đăng nhập | Lấy hồ sơ người dùng hiện tại |
| `POST` | `/api/users/refresh-token` | Public | Làm mới access token từ refresh-token cookie |
| `POST` | `/api/users/forgot-password` | Public | Yêu cầu mã đặt lại mật khẩu bằng username |
| `POST` | `/api/users/reset-password` | Public | Đặt mật khẩu mới bằng username, code, newPassword |

## Khám phá khách sạn

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `GET` | `/api/home/health` | Public | Kiểm tra API |
| `GET` | `/api/provinces` | Public | Danh sách tỉnh/thành chuẩn |
| `GET` | `/api/tags` | Public | Các tab/tag đang hoạt động, có số khách sạn |
| `GET` | `/api/tags/{slug}/hotels?page=1&pageSize=10` | Public | Khách sạn thuộc tag; trang 1-based, pageSize tối đa 100 |
| `GET` | `/api/hotels/search` | Public | Tìm kiếm khách sạn; query hỗ trợ `keyword`, `soKhach`, `ngayNhanPhong`, `ngayTraPhong`, `page`, `pageSize` |
| `GET` | `/api/hotels/{hotelId}` | Public | Chi tiết khách sạn |
| `POST` | `/api/room-types/by-hotel` | Public | Tìm loại phòng theo khách sạn và ngày lưu trú |
| `GET` | `/api/reviews/hotel/{hotelId}` | Public | Đánh giá của khách sạn |
| `GET` | `/api/banners/active` | Public | Banner đang hiển thị |

Ví dụ người dùng bấm tab Đà Lạt:

```http
GET /api/tags/da-lat/hotels?page=1&pageSize=12
```

Slug là duy nhất, chữ thường, không dấu, dùng dấu gạch nối. Danh sách tag do Admin quản lý; tag bị tắt không xuất hiện ở API public. Danh sách khách sạn và số lượng trên tab chỉ tính khách sạn ở trạng thái `DA_DUYET`.

## Booking

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `POST` | `/api/bookings` | Customer | Tạo booking |
| `GET` | `/api/bookings/history?pageIndex=1&pageSize=10` | Customer | Lịch sử booking |
| `GET` | `/api/bookings/{bookingId}` | Customer | Chi tiết booking của chính mình |
| `PUT` | `/api/bookings/{bookingId}/cancel` | Customer | Hủy booking nếu trạng thái/chính sách cho phép |

Ví dụ tạo booking:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "ngayNhanPhong": "2026-10-10T14:00:00Z",
  "ngayTraPhong": "2026-10-12T12:00:00Z",
  "phuongThucThanhToan": "cash",
  "danhSachPhong": [
    {
      "loaiPhongId": "00000000-0000-0000-0000-000000000002",
      "soLuong": 1
    }
  ]
}
```

## Thanh toán và đánh giá

| Method | Endpoint | Quyền | Mô tả |
|---|---|---|---|
| `GET` | `/api/payments/{paymentId}` | Customer | Xem payment thuộc booking của mình |
| `GET` | `/api/payments/booking/{bookingId}/qr` | Customer | Lấy thông tin QR thanh toán booking |
| `POST` | `/api/reviews` | Customer | Tạo đánh giá sau khi hoàn tất booking |
| `GET` | `/api/reviews/mine` | Customer | Danh sách đánh giá của mình |

## Quyền và lỗi

Backend xác thực quyền và quyền sở hữu dữ liệu; không tin vào `customerId` do client gửi. Lỗi thường gặp: `400` dữ liệu/trạng thái không hợp lệ, `401` chưa đăng nhập/token không hợp lệ, `403` không đủ quyền, `404` không tìm thấy tài nguyên.
