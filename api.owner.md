# Owner API

API cho chủ khách sạn. Owner chỉ được thao tác dữ liệu thuộc khách sạn của mình; backend phải xác định Owner từ JWT, không dùng OwnerId client gửi để cấp quyền.

## Cấu hình

- Base URL mặc định: `http://localhost:5295`
- Header: `Authorization: Bearer {{accessToken}}`
- JSON: `Content-Type: application/json`; upload ảnh dùng `multipart/form-data`.
- ID dùng UUID, trừ các endpoint có ID kiểu `long`.

## Khách sạn và phòng

| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/hotels` | Tạo khách sạn; multipart form gồm thông tin khách sạn và `Files` ảnh |
| `GET` | `/api/hotels/owner/search?page=1&pageSize=10` | Danh sách khách sạn của Owner hiện tại |
| `GET` | `/api/hotels/options?ownerId={ownerId}` | Option khách sạn; `ownerId` phải trùng user trong token |
| `POST` | `/api/room-types` | Tạo một hoặc nhiều loại phòng |
| `GET` | `/api/room-types/owner?khachSanId={hotelId}` | Loại phòng của khách sạn thuộc Owner |
| `POST` | `/api/rooms` | Tạo phòng |
| `GET` | `/api/rooms/by-room-type/{roomTypeId}` | Phòng theo loại phòng |
| `PUT` | `/api/rooms/{roomId}` | Cập nhật phòng |
| `DELETE` | `/api/rooms/{roomId}` | Xóa phòng |
| `GET` | `/api/rooms/prices/{roomTypeId}` | Xem giá của loại phòng |
| `POST` | `/api/rooms/prices` | Tạo/cập nhật giá phòng |
| `POST` | `/api/amenities` | Thêm tiện ích cho khách sạn |

## Booking và đánh giá

| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/bookings/owner` | Tìm booking của Owner theo bộ lọc trong request |
| `POST` | `/api/bookings/owner/all` | Danh sách booking của Owner, có phân trang |
| `POST` | `/api/bookings/owner/statistics` | Thống kê booking/doanh thu theo bộ lọc |
| `GET` | `/api/bookings/customer-options?hotelId={hotelId}` | Danh sách khách hàng đã booking khách sạn |
| `GET` | `/api/bookings/status-options` | Các trạng thái booking |
| `PUT` | `/api/bookings/{bookingId}/confirm` | Xác nhận booking |
| `PUT` | `/api/bookings/{bookingId}/check-in` | Check-in |
| `PUT` | `/api/bookings/{bookingId}/check-out` | Check-out |
| `GET` | `/api/reviews/owner/hotel/{hotelId}` | Đánh giá của khách sạn thuộc Owner |

## Tài chính và quảng cáo

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/finance/wallets` | Ví khách sạn |
| `GET` | `/api/finance/statement` | Sao kê |
| `GET` | `/api/finance/bank-accounts` | Tài khoản ngân hàng |
| `POST` | `/api/finance/bank-accounts` | Thêm tài khoản ngân hàng |
| `POST` | `/api/finance/withdrawals` | Tạo yêu cầu rút tiền |
| `GET` | `/api/finance/withdrawals` | Lịch sử yêu cầu rút tiền |
| `GET` | `/api/commissions/owner` | Hoa hồng liên quan đến Owner |
| `GET` | `/api/advertising/packages` | Gói quảng cáo khả dụng |
| `GET` | `/api/advertising/owner` | Đơn quảng cáo của Owner |
| `POST` | `/api/advertising/orders` | Tạo đơn quảng cáo |
| `PUT` | `/api/advertising/orders/{id}/confirm-payment` | Xác nhận thanh toán quảng cáo |

## Request mẫu

Tạo phòng và loại phòng nhận JSON theo DTO tương ứng; các ID khách sạn/loại phòng phải thuộc Owner hiện tại. API trả `400` khi dữ liệu hoặc chuyển trạng thái không hợp lệ, `401` khi token không hợp lệ, `403` khi tài nguyên không thuộc Owner.

`GET /api/hotels/options` hiện yêu cầu `ownerId` trùng claim hiện tại. Có thể thay contract này bằng route không nhận `ownerId` ở lần cập nhật API kế tiếp để tránh truyền dữ liệu dư thừa.
