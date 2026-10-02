# Owner API

Owner chỉ thao tác property thuộc tài khoản trong JWT. Không truyền OwnerId để tự cấp quyền; các route có kiểm tra ownership phía backend.

## Cấu hình

- Base URL mặc định: `http://localhost:5295`
- Header: `Authorization: Bearer {{ownerAccessToken}}`
- JSON: `Content-Type: application/json`; upload ảnh dùng `multipart/form-data`.
- Chạy [owner-portal.sql](owner-portal.sql) trên database trước khi dùng calendar blocks, promotions, policy/review fields. Tag dùng thêm [database-tags.sql](database-tags.sql).

## Dashboard, báo cáo và calendar

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/owner/dashboard?hotelId={id}` | Tổng booking, hôm nay/sắp tới, hoàn tất/hủy, doanh thu ngày/tháng/năm, công suất phòng hôm nay. Bỏ `hotelId` để tổng hợp mọi property |
| `GET` | `/api/owner/reports/revenue?from={ISO}&to={ISO}&groupBy=day&hotelId={id}` | Doanh thu, booking và tỷ lệ hủy theo `day`, `month` hoặc `year`; `from` inclusive, `to` exclusive |
| `GET` | `/api/owner/calendar?hotelId={id}&from={ISO}&to={ISO}` | Booking, khách liên quan, phòng và các khoảng bị chặn; tối đa 366 ngày |
| `POST` | `/api/owner/rooms/{roomId}/availability-blocks` | Chặn phòng trong khoảng thời gian |
| `DELETE` | `/api/owner/availability-blocks/{blockId}` | Gỡ block thuộc property của Owner |

Tạo block:

```json
{
  "startAt": "2026-11-01T00:00:00Z",
  "endAt": "2026-11-03T00:00:00Z",
  "reason": "Bảo trì"
}
```

Block phải ở tương lai, thời gian bắt đầu nhỏ hơn kết thúc và không được trùng booking/block khác. Availability tìm phòng của Customer cũng loại các block này.

## Property, ảnh và tiện ích

| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/hotels` | Tạo property và ảnh ban đầu; multipart form |
| `GET` | `/api/hotels/owner/search?page=1&pageSize=10` | Property của Owner |
| `GET` | `/api/hotels/options?ownerId={ownerId}` | Danh sách option; ownerId phải khớp JWT |
| `PUT` | `/api/hotels/owner/{hotelId}` | Cập nhật tên, mô tả, địa chỉ, tỉnh/thành, tọa độ, sao, chính sách hủy, check-in/out |
| `POST` | `/api/hotels/owner/{hotelId}/images` | Thêm tối đa 20 ảnh, multipart key `images` |
| `DELETE` | `/api/hotels/owner/{hotelId}/images/{imageId}` | Xóa ảnh property |
| `POST` | `/api/amenities` | Thêm tiện ích vào các property thuộc Owner |

Cập nhật property sẽ đưa trạng thái về `CHO_DUYET` để Admin duyệt lại. Chính sách và giờ nhận/trả nằm trên property; amenities hiện được thêm, nhưng chưa có endpoint Owner để xóa/sửa tiện ích đã gắn.

## Room, loại phòng và giá

| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/room-types` | Tạo loại phòng |
| `GET` | `/api/room-types/owner?khachSanId={id}` | Loại phòng của property |
| `POST` | `/api/rooms` | Tạo phòng |
| `GET` | `/api/rooms/by-room-type/{roomTypeId}` | Danh sách phòng |
| `PUT` | `/api/rooms/{roomId}` | Sửa phòng và trạng thái |
| `DELETE` | `/api/rooms/{roomId}` | Xóa phòng chưa từng booking |
| `GET` | `/api/rooms/prices/{roomTypeId}` | Xem bảng giá |
| `POST` | `/api/rooms/prices` | Thêm giá theo ngày hiệu lực |
| `POST` | `/api/room-types/by-hotel` | Tìm loại phòng public theo ngày/số khách |

Trạng thái phòng hiện dùng enum: `SAN_SANG`, `DA_DAT`, `DANG_SU_DUNG`, `BAO_TRI`, `TAM_NGUNG`. Room block theo lịch là cơ chế riêng để đánh dấu không khả dụng theo khoảng ngày.

## Booking và khách hàng

| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/bookings/owner` | Tìm booking theo property, có phân trang |
| `POST` | `/api/bookings/owner/all` | Lọc booking theo property/khách/trạng thái/ngày tạo |
| `GET` | `/api/bookings/customer-options?hotelId={id}` | Khách từng booking property |
| `GET` | `/api/bookings/status-options` | Các trạng thái booking |
| `PUT` | `/api/bookings/{bookingId}/confirm` | Chỉ xác nhận booking `CHO_XAC_NHAN` |
| `PUT` | `/api/bookings/{bookingId}/reject` | Từ chối booking đang chờ, chuyển `TU_CHOI` |
| `PUT` | `/api/bookings/{bookingId}/owner-cancel` | Hủy booking `DA_XAC_NHAN`, chuyển `DA_HUY` |
| `PUT` | `/api/bookings/{bookingId}/check-in` | Check-in booking đã xác nhận, đúng ngày trở đi |
| `PUT` | `/api/bookings/{bookingId}/check-out` | Checkout booking đang `DA_CHECK_IN` |
| `POST` | `/api/bookings/owner/statistics` | Thống kê booking/doanh thu theo khoảng thời gian |

## Promotion, review và tài chính

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/owner/promotions?hotelId={id}` | Promotion của property |
| `POST` | `/api/owner/promotions?hotelId={id}` | Tạo coupon `PERCENT` hoặc `FIXED` |
| `PUT` | `/api/owner/promotions/{promotionId}` | Cập nhật coupon/điều kiện |
| `DELETE` | `/api/owner/promotions/{promotionId}` | Tắt coupon mềm |
| `GET` | `/api/reviews/owner/hotel/{hotelId}` | Review property |
| `PUT` | `/api/reviews/owner/{reviewId}/response` | Phản hồi review |
| `POST` | `/api/reviews/owner/{reviewId}/report` | Báo cáo review lên Admin |
| `GET` | `/api/finance/wallets` | Ví property |
| `GET` | `/api/finance/statement?from={ISO}&to={ISO}&khachSanId={id}` | Sao kê |
| `GET` | `/api/finance/bank-accounts` | Tài khoản nhận tiền |
| `POST` | `/api/finance/bank-accounts` | Thêm tài khoản nhận tiền |
| `POST` | `/api/finance/withdrawals` | Yêu cầu rút tiền |
| `GET` | `/api/finance/withdrawals` | Lịch sử rút tiền |
| `GET` | `/api/commissions/owner` | Hoa hồng |
| `GET` | `/api/advertising/packages` | Gói quảng cáo |
| `GET` | `/api/advertising/owner` | Đơn quảng cáo |
| `POST` | `/api/advertising/orders` | Tạo đơn quảng cáo |

Coupon hỗ trợ thời hạn, mức đơn tối thiểu, giới hạn giảm tối đa. Revenue report ghi nhận doanh thu từ booking đã checkout. Staff chưa được hỗ trợ: hiện chưa có bảng gán nhân viên theo property và authorization theo phạm vi property, nên chưa có API tạo/phân quyền Staff an toàn.

## Realtime và giới hạn hiện tại

SignalR hub `/bookingHub` phát event `OwnerBookingCreated` cho booking mới và `BookingStatusChanged` cho thay đổi booking. Chưa có notification inbox/history; thanh toán, review và duyệt property chưa phát event Owner. Rate theo mùa/ngày đã lưu qua bảng giá, nhưng booking hiện lấy một giá hiện hành cho toàn bộ đêm lưu trú; cần hoàn thiện tính giá theo từng đêm trước khi coi seasonal pricing là đầy đủ.
