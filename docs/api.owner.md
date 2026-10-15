# Owner API

Tài liệu hợp đồng các endpoint Owner hiện có trong backend. Base URL local mặc định là `http://localhost:5295`; thay bằng URL của môi trường đang chạy.

## Quy ước chung

- Request/response dùng JSON UTF-8; tên thuộc tính JSON là `camelCase`. Tạo khách sạn và ảnh dùng `multipart/form-data`.
- API có quyền Owner yêu cầu `Authorization: Bearer <accessToken>`. Backend lấy Owner ID từ token và kiểm tra quyền sở hữu khách sạn/phòng; không dùng ID do client truyền để cấp quyền.
- UUID dùng cho khách sạn, booking, loại phòng, phòng, review và promotion; một số ID như ảnh, yêu cầu rút tiền và gói quảng cáo là số nguyên.
- Ngày giờ truyền theo ISO 8601. Các trang bắt đầu từ `1` trừ khi endpoint ghi chú rõ khác.

## Khách sạn

| Method và endpoint | Mô tả / tham số |
|---|---|
| `POST /api/hotels` | Tạo khách sạn. `multipart/form-data`: `TenKhachSan`, `MoTa`, `DiaChi`, `ChinhSachHuy`, `ThanhPho`, `ViDo`, `KinhDo`, `SoSao`, `GioNhanPhong`, `GioTraPhong`, `Files` (một hoặc nhiều ảnh), `TagIds` (UUID tag, lặp lại field cho mỗi tag, tùy chọn). Chỉ nhận tag đang hoạt động; tag không hợp lệ trả `400`. Owner lấy từ token; không cần gửi `NguoiTao` để xác định quyền. |
| `GET /api/hotels/owner/search?page=1&pageSize=10` | Danh sách khách sạn thuộc Owner hiện tại, sắp xếp mới tạo trước. Query `page`, `pageSize`; mặc định `1`, `10`. Trả `{ data, totalRow, totalPage }`; mỗi phần tử có `id`, `tenKhachSan`, `moTa`, `diaChi`, `soSao`, `trangThai`, `ngayTao`, `gia`, `urls`. |
| `GET /api/hotels/options?ownerId={ownerId}` | Danh sách option khách sạn thuộc Owner; bắt buộc `ownerId` và phải khớp ID trong token, sai khác trả `401`. Trả mảng `{ id, name }`. |
| `PUT /api/hotels/owner/{hotelId}` | Cập nhật thông tin khách sạn bằng JSON xem ví dụ bên dưới. Nếu lưu thành công trạng thái được đưa về `CHO_DUYET` để Admin duyệt lại. |
| `POST /api/hotels/owner/{hotelId}/images` | Thêm ảnh bằng `multipart/form-data`, field `images` lặp lại cho từng file; service giới hạn tối đa 20 ảnh trong một lần gọi. |
| `DELETE /api/hotels/owner/{hotelId}/images/{imageId}` | Xóa ảnh thuộc khách sạn; thành công `204`, không tìm thấy/không thuộc Owner `404`. |
| `POST /api/amenities` | Thêm tiện ích theo `CreateTienIchRequest`; khách sạn phải thuộc Owner. Xem DTO/backend để biết thuộc tính cụ thể. |

Body cập nhật khách sạn:

```json
{
  "tenKhachSan": "Central Hotel",
  "moTa": "Mô tả",
  "chinhSachHuy": "Hủy trước ngày nhận phòng",
  "diaChi": "1 phố Mẫu",
  "thanhPho": "Hà Nội",
  "viDo": 21.028,
  "kinhDo": 105.854,
  "soSao": 4,
  "gioNhanPhong": "14:00",
  "gioTraPhong": "12:00"
}
```

Tên, địa chỉ, thành phố và giờ nhận/trả là bắt buộc; `soSao` từ `1` đến `5`. Khách sạn không thuộc Owner hoặc body không hợp lệ trả `400`.

## Loại phòng, phòng và giá

| Method và endpoint | Hợp đồng |
|---|---|
| `POST /api/room-types` | Tạo nhiều loại phòng cùng lúc; body là mảng `{ "khachSanId": "<uuid>", "tenLoaiPhong": "...", "soKhachToiDa": 2, "kieuGiuong": "...", "moTa": "..." }`. Khách sạn phải thuộc Owner. |
| `GET /api/room-types/owner?khachSanId={hotelId}` | Danh sách loại phòng trong khách sạn. |
| `POST /api/rooms` | Tạo phòng bằng JSON `{ "loaiPhongId": "<uuid>", "soPhong": "...", "tang": 1, "trangThai": "SAN_SANG" }`. |
| `GET /api/rooms/by-room-type/{roomTypeId}` | Danh sách phòng của loại phòng thuộc Owner hiện tại. |
| `PUT /api/rooms/{roomId}` | Cập nhật phòng bằng JSON `{ "soPhong": "...", "tang": 1, "trangThai": "SAN_SANG" }`; không tồn tại/không thuộc Owner trả `404`. |
| `DELETE /api/rooms/{roomId}` | Xóa phòng; phòng đã phát sinh booking hoặc không thuộc Owner trả `400`; thành công `204`. |
| `GET /api/rooms/prices/{roomTypeId}` | Lấy bảng giá của loại phòng thuộc Owner. |
| `POST /api/rooms/prices` | Tạo giá: `{ "loaiPhongId": "<uuid>", "gia": 1000000, "ngayBatDau": "2026-10-01T00:00:00Z", "ngayKetThuc": "2026-11-01T00:00:00Z" }`. `ngayKetThuc` có thể null. |
| `POST /api/room-types/by-hotel` | Endpoint public tìm loại phòng theo body `{ "khachSanId": "<uuid>", "soKhach": 2, "ngayNhan": "2026-10-10T00:00:00Z", "ngayTra": "2026-10-12T00:00:00Z" }`; dùng để xem còn phòng khi đặt. |

Trạng thái phòng trong enum hiện có: `SAN_SANG`, `DA_DAT`, `DANG_SU_DUNG`, `BAO_TRI`, `TAM_NGUNG`. Tình trạng chặn theo ngày là dữ liệu lịch riêng, không phải trạng thái phòng.

## Booking và filter options

| Method và endpoint | Hợp đồng |
|---|---|
| `POST /api/bookings/owner` | Body `{ "khachSanId": "<uuid>", "maDatPhong": "<mã booking tùy chọn>", "page": 1, "pageSize": 10 }`; danh sách booking cho khách sạn thuộc Owner, có thể tìm theo một phần mã. Mỗi booking trong danh sách có thêm `maDatPhong`. |
| `POST /api/bookings/owner/all` | Body `{ "hotelId": "<uuid|null>", "customerId": "<uuid|null>", "trangThai": "<string|null>", "ngayTao": "2026-10-01T00:00:00Z", "maDatPhong": "<mã booking tùy chọn>", "page": 1, "pageSize": 10 }`; lọc booking theo các trường body, tìm theo một phần mã; mỗi booking có thêm `maDatPhong`. |
| `GET /api/bookings/customer-options?hotelId={hotelId}` | Danh sách khách hàng đã đặt khách sạn, dùng làm lựa chọn filter. Khách sạn phải thuộc Owner. |
| `GET /api/bookings/status-options` | Các lựa chọn trạng thái booking dạng `{ "id": "CHO_XAC_NHAN", "name": "CHO_XAC_NHAN" }`. |
| `POST /api/bookings/owner/statistics` | Body `{ "hotelId": "<uuid>", "startDate": "2026-10-01T00:00:00Z", "endDate": "2026-11-01T00:00:00Z", "status": null }`; trả `{ datPhong, tongTien }`. |
| `PUT /api/bookings/{bookingId}/confirm` | Xác nhận booking đang chờ; Owner được xác thực từ token. |
| `PUT /api/bookings/{bookingId}/reject` | Từ chối booking đang chờ; không đúng Owner/trạng thái trả `400`. |
| `PUT /api/bookings/{bookingId}/owner-cancel` | Owner hủy booking đã xác nhận; không hợp lệ trả `400`. |
| `PUT /api/bookings/{bookingId}/check-in` | Check-in booking thuộc Owner. |
| `PUT /api/bookings/{bookingId}/check-out` | Check-out booking thuộc Owner. |

Enum trạng thái booking: `CHO_XAC_NHAN`, `DA_XAC_NHAN`, `DA_CHECK_IN`, `DA_CHECK_OUT`, `DA_HUY`, `TU_CHOI`, `KHONG_DEN`. Với filter dropdown, gọi `status-options` thay vì tự tạo nhãn; giá trị gửi vào body filter là chuỗi `id`.

## Dashboard, calendar và chặn phòng

| Method và endpoint | Tham số / hành vi |
|---|---|
| `GET /api/owner/dashboard?hotelId={uuid}` | Dashboard một khách sạn của Owner. Bỏ `hotelId` để tổng hợp mọi khách sạn. Không sở hữu khách sạn trả `403`. |
| `GET /api/owner/reports/revenue?from={ISO}&to={ISO}&groupBy=day&hotelId={uuid}` | Báo cáo doanh thu. `from`/`to` bắt buộc; `groupBy` là `day`, `month` hoặc `year`, mặc định `day`; `hotelId` tùy chọn. Khoảng thời gian không hợp lệ trả `400`. |
| `GET /api/owner/calendar?hotelId={uuid}&from={ISO}&to={ISO}` | Lịch booking/phòng/block trong khoảng ngày; đủ cả ba query. Khoảng tối đa 366 ngày. Booking trong lịch có thêm `maDatPhong`. |
| `POST /api/owner/rooms/{roomId}/availability-blocks` | Tạo block phòng với body `{ "startAt": "2026-11-01T00:00:00Z", "endAt": "2026-11-03T00:00:00Z", "reason": "Bảo trì" }`. Khoảng phải hợp lệ, ở tương lai và không trùng booking/block. |
| `DELETE /api/owner/availability-blocks/{blockId}` | Xóa block của Owner; thành công `204`, không tìm thấy `404`. |

## Promotion, review, tài chính và quảng cáo

| Method và endpoint | Hợp đồng |
|---|---|
| `GET /api/owner/promotions?hotelId={uuid}` | Danh sách promotion của khách sạn; khách sạn không thuộc Owner trả `403`. |
| `POST /api/owner/promotions?hotelId={uuid}` | Tạo promotion bằng JSON `{ "code": "DALAT10", "name": "Giảm 10%", "discountType": "PERCENT", "discountValue": 10, "minBookingAmount": 500000, "maxDiscountAmount": 200000, "startsAt": "2026-10-01T00:00:00Z", "endsAt": "2026-12-01T00:00:00Z" }`. Kiểu giảm hỗ trợ `PERCENT` hoặc `FIXED`; sai điều kiện trả `400`. |
| `PUT /api/owner/promotions/{promotionId}` | Cập nhật promotion; dùng cùng cấu trúc body với tạo. |
| `DELETE /api/owner/promotions/{promotionId}` | Tắt promotion mềm; thành công `204`. |
| `GET /api/reviews/owner/hotel/{hotelId}` | Review của khách sạn thuộc Owner. |
| `PUT /api/reviews/owner/{reviewId}/response` | Body `{ "response": "Cảm ơn bạn đã đánh giá." }`; lưu phản hồi, không thuộc Owner trả `404`. |
| `POST /api/reviews/owner/{reviewId}/report` | Body `{ "reason": "Nội dung vi phạm" }`; báo cáo review, thành công `202 Accepted`. |
| `GET /api/finance/wallets` | Ví của Owner hiện tại. |
| `GET /api/finance/statement?from={ISO}&to={ISO}&khachSanId={uuid}` | Sao kê; query ngày và khách sạn tùy chọn. |
| `GET /api/finance/bank-accounts` | Tài khoản ngân hàng của Owner. |
| `POST /api/finance/bank-accounts` | Tạo tài khoản `{ "tenNganHang": "...", "soTaiKhoan": "...", "chuTaiKhoan": "...", "chiNhanh": null, "qrCode": null, "isDefault": true }`. |
| `POST /api/finance/withdrawals` | Yêu cầu rút tiền `{ "khachSanId": "<uuid>", "taiKhoanNganHangId": "<uuid>", "soTien": 1000000, "ghiChu": null }`. |
| `GET /api/finance/withdrawals` | Lịch sử yêu cầu rút tiền của Owner. |
| `GET /api/commissions/owner` | Hoa hồng của Owner. |
| `GET /api/advertising/packages` | Gói quảng cáo khả dụng; public, không cần token. |
| `GET /api/advertising/owner` | Chiến dịch quảng cáo của Owner. |
| `POST /api/advertising/orders` | Tạo đơn `{ "khachSanId": "<uuid>", "goiQuangCaoId": 1 }`. |
| `PUT /api/advertising/orders/{id}/confirm-payment` | Xác nhận thanh toán đơn của Owner; đơn không ở trạng thái chờ thanh toán trả `400`. |

## Lỗi, realtime và giới hạn

- `400`: dữ liệu/trạng thái không hợp lệ; `401`: thiếu/sai token; `403`: không thuộc phạm vi Owner; `404`: không tìm thấy tài nguyên; `202`: tiếp nhận báo cáo; `204`: thành công, không body.
- SignalR hub `/bookingHub` có các event booking `OwnerBookingCreated` và `BookingStatusChanged`.
- Chưa có API quản lý Staff theo phạm vi khách sạn. Không có endpoint Owner để xóa/sửa tiện ích đã gắn.
