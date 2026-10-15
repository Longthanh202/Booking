# Customer API

Tài liệu hợp đồng các endpoint Customer/public hiện có trong backend. Base URL local mặc định là `http://localhost:5295`; thay bằng URL của môi trường đang chạy.

## Quy ước chung

- Request/response dùng JSON UTF-8; tên thuộc tính JSON là `camelCase`. Ngày giờ gửi theo ISO 8601.
- Endpoint `Customer` cần `Authorization: Bearer <accessToken>`. Endpoint public không bắt buộc đăng nhập.
- UUID dùng cho khách sạn, loại phòng, booking, payment và review.

## Tài khoản và xác thực

| Method và endpoint | Quyền | Request và response |
|---|---|---|
| `POST /api/users/register` | Public | JSON `{ "username": "...", "password": "...", "fullName": "...", "phone": "...", "address": "...", "email": "...", "createBy": null }`. Thành công trả `200` với dữ liệu request đã gửi; thất bại trả `400`. |
| `POST /api/users/login` | Public | JSON `{ "username": "...", "password": "..." }`. Thành công trả `{ "status": true, "token": "<JWT>", "message": "..." }`; sai thông tin trả `400`. Endpoint có rate limit đăng nhập. |
| `POST /api/auth/google-login` | Public | JSON `{ "idToken": "<Google ID token>" }`; token rỗng trả `400`. |
| `GET /api/users/me` | Đã đăng nhập | Trả hồ sơ của user trong token; không nhận user ID từ client. |
| `POST /api/users/refresh-token` | Public/cookie | Không có body; dùng refresh-token cookie hiện có. Trả chuỗi JSON access token mới; cookie không hợp lệ trả `401`. |
| `POST /api/users/forgot-password` | Public | JSON `{ "username": "..." }`. Thành công trả message chung để không tiết lộ tài khoản có tồn tại hay không. |
| `POST /api/users/reset-password` | Public | JSON `{ "username": "...", "code": "...", "newPassword": "..." }`. Các trường đều bắt buộc; thiếu trường trả `400`. |

## Khám phá khách sạn và dữ liệu filter

| Method và endpoint | Quyền | Hợp đồng |
|---|---|---|
| `GET /api/home/health` | Public | Health check; thành công `200`. |
| `GET /api/provinces` | Public | Danh sách tỉnh/thành; phần tử có `code`, `name`, `name_en`, `full_name`, `full_name_en`, `code_name`, `administrative_unit_id`. |
| `GET /api/tags` | Public | Danh sách tag đang hoạt động, có `id`, `name`, `slug`, `description`, `sortOrder`, `isActive`, `hotelCount`. |
| `GET /api/tags/{slug}/hotels?page=1&pageSize=10` | Public | Danh sách khách sạn đã duyệt thuộc tag. `page >= 1`, `pageSize` từ `1` đến `100`. Trả `{ tag, data, totalRow, totalPage, page, pageSize }`; slug không tồn tại/đã ẩn trả `404`. |
| `GET /api/hotels/search` | Public | Query `keyword`, `soKhach`, `ngayNhanPhong`, `ngayTraPhong`, `page` (mặc định `1`), `pageSize` (mặc định `10`). Ngày nhận/trả dùng ISO 8601. `page` và `pageSize` phải lớn hơn `0`, nếu không trả `400`. |
| `GET /api/hotels/{hotelId}` | Public | Chi tiết khách sạn gồm `id`, tên/mô tả/địa chỉ/chính sách, sao, giờ nhận/trả, tên thành phố, tiện ích và ảnh. |
| `POST /api/room-types/by-hotel` | Public | Tìm loại phòng theo body `{ "khachSanId": "<uuid>", "soKhach": 2, "ngayNhan": "2026-10-10T00:00:00Z", "ngayTra": "2026-10-12T00:00:00Z" }`. Kết quả rỗng/đầu vào không hợp lệ trả `400`. |
| `GET /api/reviews/hotel/{hotelId}` | Public | Danh sách đánh giá của khách sạn. |
| `GET /api/banners/active` | Public | Danh sách banner đang hoạt động. |

Ví dụ tìm khách sạn:

```http
GET /api/hotels/search?keyword=Central&soKhach=2&ngayNhanPhong=2026-10-10T00%3A00%3A00Z&ngayTraPhong=2026-10-12T00%3A00%3A00Z&page=1&pageSize=12
```

Filter danh sách admin khách sạn và các enum trạng thái được mô tả trong [api.admin.md](api.admin.md). API tìm kiếm public ở trên chỉ khai báo `keyword`, `soKhach`, `ngayNhanPhong`, `ngayTraPhong`, `page`, `pageSize`; không gửi các tham số lọc chỉ có ở admin.

## Booking

| Method và endpoint | Quyền | Request và response |
|---|---|---|
| `POST /api/bookings` | Customer | Tạo booking bằng body mẫu bên dưới. Thành công trả `{ "message": "Đặt phòng thành công", "result": { "id": "<uuid>", "maDatPhong": "<mã do database sinh>", "trangThai": "..." } }`; dữ liệu/điều kiện promotion không hợp lệ trả `400`; phòng đang được xử lý booking khác trả `409`. |
| `GET /api/bookings/history?pageIndex=1&pageSize=10&maDatPhong={mã}` | Customer | Lịch sử booking của user hiện tại, mỗi booking có `maDatPhong`; có thể truyền `maDatPhong` để tìm theo một phần mã. Gửi `pageIndex`, `pageSize`; backend nhận kiểu integer, nên truyền giá trị dương. |
| `GET /api/bookings/{bookingId}` | Customer | Chi tiết booking của chính user; không tìm thấy hoặc không thuộc user trả `404`. |
| `PUT /api/bookings/{bookingId}/cancel` | Customer | Hủy booking nếu trạng thái/nghiệp vụ cho phép; thành công trả message, nếu không thể hủy trả `400`. |

Body tạo booking:

```json
{
  "khachSanId": "00000000-0000-0000-0000-000000000001",
  "ngayNhanPhong": "2026-10-10T14:00:00Z",
  "ngayTraPhong": "2026-10-12T12:00:00Z",
  "phuongThucThanhToan": "cash",
  "maKhuyenMai": null,
  "danhSachPhong": [
    {
      "loaiPhongId": "00000000-0000-0000-0000-000000000002",
      "soLuong": 1
    }
  ]
}
```

`maKhuyenMai` tùy chọn. Mỗi dòng phòng gồm ID loại phòng và số lượng cần đặt. Không gửi `customerId`; backend xác định khách từ access token.

Backend dùng Redis khóa loại phòng trong lúc kiểm tra phòng trống và lưu booking để các request đồng thời không gán cùng phòng. Khóa tự hết hạn sau 2 phút nếu tiến trình không giải phóng được.

## Thanh toán và đánh giá

| Method và endpoint | Quyền | Mô tả |
|---|---|---|
| `GET /api/payments/{paymentId}` | Customer | Xem payment thuộc booking của user; không thuộc user/không tồn tại trả `404`. |
| `GET /api/payments/booking/{bookingId}/qr` | Customer | Lấy QR/thông tin thanh toán của booking; không có khoản chờ thanh toán hoặc tài khoản nhận tiền mặc định trả `404`. |
| `POST /api/reviews` | Customer | Tạo review bằng body `{ "khachSanId": "<uuid>", "soSao": 5, "noiDung": "..." }`. Chỉ được đánh giá sau khi hoàn tất booking; dữ liệu sai trả `400`. |
| `GET /api/reviews/mine` | Customer | Danh sách review của user hiện tại. |

## Lỗi và realtime

- `400`: body/điều kiện nghiệp vụ không hợp lệ; `401`: thiếu/sai token; `403`: không đủ quyền; `404`: không tìm thấy tài nguyên hoặc tài nguyên không thuộc user.
- SignalR hub `/bookingHub` có thể phát `BookingSuccess` và `BookingStatusChanged` cho client đã xác thực.
- Backend kiểm tra quyền sở hữu booking/payment; không dựa vào `customerId` do client gửi.
