# Admin API

Tài liệu hợp đồng các endpoint Admin hiện có trong backend. Base URL local mặc định là `http://localhost:5295`; thay bằng URL của môi trường đang chạy.

## Quy ước chung

- Request/response dùng JSON UTF-8; tên thuộc tính JSON là `camelCase`. Endpoint upload dùng `multipart/form-data`.
- Endpoint ghi quyền `Admin` cần gửi `Authorization: Bearer <accessToken>`. Không gửi dấu ngoặc nhọn trong token.
- ID khách sạn, user, booking, payment và tag là UUID; ID banner, yêu cầu rút tiền và quảng cáo là số nguyên.
- Query/body không nêu trong bảng không được dùng làm bộ lọc. Các API lựa chọn chưa có endpoint thì được ghi rõ bên dưới; không tự giả định có route `/options`.

## Quản lý khách sạn

### Lấy danh sách khách sạn

`GET /api/hotels/admin/search` — yêu cầu role `Admin`.

| Query | Kiểu | Mặc định | Ý nghĩa |
|---|---|---:|---|
| `keyword` | string | bỏ trống | Tìm theo tên khách sạn (`TenKhachSan` contains). |
| `thanhPho` | string | bỏ trống | Lọc thành phố chính xác; giá trị phải khớp chuỗi `ThanhPho` lưu ở khách sạn. |
| `soSao` | integer | `0` | `0` = tất cả; giá trị dương lọc sao bằng chính xác giá trị đó (thực tế thường `1`–`5`). |
| `trangThai` | string | bỏ trống | Lọc khớp chính xác trạng thái. Xem danh sách trạng thái bên dưới. |
| `page` | integer | `1` | Trang bắt đầu từ `1`; giá trị nhỏ hơn hoặc bằng 0 được thay bằng `1`. |
| `pageSize` | integer | `10` | Số dòng/trang; giá trị nhỏ hơn hoặc bằng 0 được thay bằng `10`. |
| `viDo`, `kinhDo` | number | `0` | Được nhận bởi API nhưng hiện **chưa được repository dùng để lọc**. Không dùng hai tham số này để kỳ vọng lọc vị trí/bán kính. |

Ví dụ:

```http
GET /api/hotels/admin/search?keyword=Central&thanhPho=H%C3%A0%20N%E1%BB%99i&soSao=4&trangThai=CHO_DUYET&page=1&pageSize=20
Authorization: Bearer <accessToken>
```

Response `200 OK`:

```json
{
  "data": [
    {
      "id": "00000000-0000-0000-0000-000000000001",
      "tenKhachSan": "Central Hotel",
      "moTa": "Mô tả khách sạn",
      "diaChi": "1 phố Mẫu",
      "soSao": 4,
      "trangThai": "CHO_DUYET",
      "ngayTao": "2026-10-01T10:00:00",
      "gia": null,
      "urls": ["https://example.com/hotel.jpg"]
    }
  ],
  "totalRow": 1,
  "totalPage": 1
}
```

`gia` là nullable và có thể không được thiết lập trong kết quả admin. Khi không có kết quả, `data` rỗng, `totalRow` và `totalPage` bằng `0`.

### Lựa chọn cho các bộ lọc khách sạn

Backend hiện **không có API trả options trạng thái khách sạn**. Dùng các giá trị enum sau trong query `trangThai` và body cập nhật trạng thái:

| Giá trị | Ý nghĩa giao diện gợi ý |
|---|---|
| `CHO_DUYET` | Chờ duyệt |
| `DA_DUYET` | Đã duyệt |
| `TAM_NGUNG` | Tạm ngưng |
| `TU_CHOI` | Từ chối |
| `DA_KHOA` | Đã khóa |

Đây là lựa chọn cố định từ enum backend, không phải một API options. Sao có thể dựng từ `1` đến `5` (gửi `soSao=0` để bỏ lọc). Danh sách tỉnh/thành có thể tải bằng `GET /api/provinces` (public); endpoint này trả danh sách bản ghi tỉnh/thành, trong đó có các trường `code`, `name`, `name_en`, `full_name`, `full_name_en`, `code_name`, `administrative_unit_id`. Bộ lọc `thanhPho` cần gửi đúng chuỗi đang lưu ở `KhachSan.ThanhPho`; endpoint lọc hiện không nhận mã tỉnh.

### Cập nhật trạng thái khách sạn

`PUT /api/hotels/admin/{hotelId}/status` — yêu cầu role `Admin`.

```json
{ "trangThai": "DA_DUYET" }
```

Thành công trả `200` với `{ "message": "Cập nhật trạng thái property thành công." }`. Trạng thái rỗng, khách sạn không tồn tại hoặc giá trị không hợp lệ trả `400`. Backend nhận chuỗi trạng thái; nên chỉ gửi một trong năm giá trị enum ở bảng trên.

## Quản lý tài khoản và phân quyền

| Method và endpoint | Quyền | Tham số/body và kết quả |
|---|---|---|
| `GET /api/users/admin` | Admin | Query: `keyword` (tìm username/tên/email), `status` (`0` hoạt động, `1` khóa; bỏ trống = tất cả), `page` mặc định `1`, `pageSize` mặc định `20`, tối đa `100`. Trả `{ data: [{ userId, username, fullName, email, phone, roleName, isDel, createAt }], totalRow, totalPage, page, pageSize }`. |
| `PUT /api/users/admin/{userId}/status` | Admin | Body `{ "isDel": 0 }` mở khóa, `{ "isDel": 1 }` khóa. Thành công trả message; không tìm thấy user trả `404`. Không thể tự khóa tài khoản Admin đang đăng nhập (`400`). |
| `POST /api/users/role-permissions` | Admin | Body theo `RolePermissionRequest`: `{ "roleId": "<uuid>", "permissions": [...] }`; mỗi phần tử theo `RolePermissionDto`. Thành công `200`. Không có API gán role trực tiếp cho user trong controller hiện tại. |

Ví dụ lọc user:

```http
GET /api/users/admin?keyword=nguyen&status=0&page=1&pageSize=20
```

`page <= 0`, `pageSize <= 0`, `pageSize > 100` hoặc `status` ngoài `0/1` trả `400`.

## Booking, payment và tài chính

| Method và endpoint | Quyền | Mô tả / hợp đồng |
|---|---|---|
| `POST /api/bookings/admin/search` | Admin | Body là `OwnerBookingListRequest`: `{ "hotelId": "<uuid|null>", "customerId": "<uuid|null>", "trangThai": "<string|null>", "ngayTao": "<ISO-8601>", "maDatPhong": "<mã booking tùy chọn>", "page": 1, "pageSize": 10 }`. Có thể tìm theo một phần mã; mỗi booking trong danh sách có thêm `maDatPhong`. |
| `GET /api/payments/admin` | Admin | Trả danh sách payment toàn hệ thống; không nhận query filter/phân trang. |
| `PUT /api/payments/admin/{paymentId}/status` | Admin | Body `{ "trangThai": "<trạng thái payment>" }`. Trạng thái phải hợp lệ theo nghiệp vụ; payment không tồn tại hoặc giá trị không hợp lệ trả `400`. |
| `GET /api/finance/admin/withdrawals/pending` | Admin | Danh sách yêu cầu rút tiền chờ xử lý; không nhận query. |
| `PUT /api/finance/admin/withdrawals/{id}` | Admin | Body `{ "trangThai": "<trạng thái>", "maGiaoDich": "<string|null>", "ghiChu": "<string|null>" }`. Yêu cầu không tồn tại hoặc trạng thái không hợp lệ trả `400`. |
| `GET /api/commissions/admin/pending` | Admin | Danh sách hoa hồng chờ xử lý; không nhận query. |

Các giá trị enum trạng thái payment hiện có: `CHO_THANH_TOAN`, `DA_THANH_TOAN`, `DA_HUY`, `DA_HOAN_TIEN`. Các giá trị enum trạng thái chi trả có ở code: `CHO_CHI_TRA`, `DANG_XU_LY`, `DA_CHI_TRA`, `THAT_BAI`, `DA_HUY`. Hai danh sách này hiện không có API options riêng.

## Quảng cáo

| Method và endpoint | Quyền | Mô tả / hợp đồng |
|---|---|---|
| `GET /api/advertising/admin/pending` | Admin | Danh sách chiến dịch chờ duyệt. |
| `PUT /api/advertising/admin/{id}/approve` | Admin | Body `{ "approved": true, "lyDo": null }` để duyệt; đặt `approved: false` và cung cấp `lyDo` khi từ chối. Chiến dịch không hợp lệ/đã xử lý trả `400`. |

## Banner

| Method và endpoint | Quyền | Mô tả / hợp đồng |
|---|---|---|
| `GET /api/banners` | Admin | Query `keyword` (tùy chọn), `isActive` mặc định `0` (`0` = không lọc trạng thái, `1` = chỉ banner đang bật), `startRow` mặc định `0`, `endRow` mặc định `50`; trả các dòng trong đoạn `[startRow, endRow)`. Không có giá trị để chỉ lấy banner tắt. `startRow >= 0`, `endRow > startRow`. |
| `POST /api/banners` | Admin | `multipart/form-data`: `Title`, `Subtitle`, `IsActive` (`0/1`), `File` (ảnh). Trả đối tượng banner mới. |
| `PUT /api/banners/{id}` | Admin | JSON `{ "title": "...", "subtitle": "...", "url": "...", "isActive": 1 }`. Không tìm thấy banner hoặc body không hợp lệ trả `400`. |
| `DELETE /api/banners/{id}` | Admin | Xóa banner; thành công `204 No Content`, không tìm thấy `404`. |

Public có thể đọc banner đang hoạt động bằng `GET /api/banners/active`.

## Đánh giá và tag

| Method và endpoint | Quyền | Mô tả / hợp đồng |
|---|---|---|
| `DELETE /api/reviews/{reviewId}` | Admin | Xóa review; thành công `204`, không tìm thấy `404`. |
| `GET /api/tags/admin` | Admin | Danh sách tag bao gồm trạng thái bật/tắt. |
| `POST /api/tags/admin` | Admin | Body `{ "name": "...", "slug": "...", "description": null, "sortOrder": 1, "isActive": true }`. Tên tối đa 100 ký tự, slug tối đa 120; slug duy nhất. Thành công `201`. |
| `PUT /api/tags/admin/{tagId}` | Admin | Cùng body với tạo; thành công `200`. |
| `PUT /api/tags/admin/{tagId}/hotels` | Admin | Thay thế toàn bộ liên kết khách sạn bằng body `{ "hotelIds": ["<uuid>"] }`; danh sách rỗng dùng để gỡ hết. Thành công `204`. |
| `DELETE /api/tags/admin/{tagId}` | Admin | Tắt tag mềm; thành công `204`, không tìm thấy `404`. |
| `GET /api/tags` | Public | Các tag đang hoạt động để dựng tab khám phá. |
| `GET /api/tags/{slug}/hotels?page=1&pageSize=10` | Public | Khách sạn đã duyệt thuộc tag; `pageSize` từ `1` đến `100`. Tag không tồn tại/đã ẩn trả `404`. |

## Lỗi và giới hạn đã biết

- `400`: query/body/trạng thái không hợp lệ hoặc nghiệp vụ không cho phép.
- `401`: thiếu/sai token; `403`: token không có role Admin; `404`: tài nguyên không tồn tại; `204`: thao tác thành công không có response body.
- Backend hiện chưa có dashboard admin, audit log, refund, luồng duyệt Owner riêng hoặc API options trạng thái khách sạn. Không gọi các route chưa được liệt kê ở đây nếu chưa triển khai.
