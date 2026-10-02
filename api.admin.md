# Admin API

Admin quản trị dữ liệu toàn hệ thống. Các route dưới đây là endpoint đang có trong API; route gắn `/admin` yêu cầu JWT có role `Admin`. Backend vẫn cần kiểm tra dữ liệu đầu vào và ghi nhận các thao tác nhạy cảm.

## Cấu hình

- Base URL mặc định: `http://localhost:5295`
- Header: `Authorization: Bearer {{adminAccessToken}}`
- JSON: `Content-Type: application/json`; tạo banner dùng `multipart/form-data`.

## Tổng quan và dữ liệu vận hành

| Method | Endpoint | Mô tả |
|---|---|---|
| `POST` | `/api/bookings/admin/search` | Tìm booking toàn hệ thống theo `OwnerBookingListRequest` |
| `GET` | `/api/payments/admin` | Danh sách payment toàn hệ thống |
| `PUT` | `/api/payments/admin/{paymentId}/status` | Cập nhật trạng thái payment |
| `GET` | `/api/finance/admin/withdrawals/pending` | Yêu cầu rút tiền chờ xử lý |
| `PUT` | `/api/finance/admin/withdrawals/{id}` | Xử lý yêu cầu rút tiền |
| `GET` | `/api/commissions/admin/pending` | Hoa hồng chờ xử lý |
| `GET` | `/api/advertising/admin/pending` | Đơn quảng cáo chờ duyệt |
| `PUT` | `/api/advertising/admin/{id}/approve` | Duyệt đơn quảng cáo |
| `GET` | `/api/users/admin?keyword={keyword}&status=0&page=1&pageSize=20` | Tìm user theo username, tên, email; status `0` hoạt động, `1` bị khóa |
| `PUT` | `/api/users/admin/{userId}/status` | Khóa/mở khóa user bằng `IsDel` (`1` khóa, `0` mở) |

Body cập nhật trạng thái user:

```json
{
  "isDel": 1
}
```

## Quản lý khách sạn và nội dung

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/hotels/admin/search` | Tìm khách sạn theo keyword, thành phố, tọa độ, số sao, trạng thái và phân trang |
| `PUT` | `/api/hotels/admin/{hotelId}/status` | Cập nhật trạng thái khách sạn |
| `GET` | `/api/banners?keyword={keyword}&isActive=0&startRow=0&endRow=50` | Tìm banner, phân trang theo row |
| `POST` | `/api/banners` | Tạo banner bằng multipart form |
| `PUT` | `/api/banners/{id}` | Cập nhật banner |
| `DELETE` | `/api/banners/{id}` | Xóa banner |
| `DELETE` | `/api/reviews/{reviewId}` | Xóa đánh giá vi phạm |
| `POST` | `/api/users/role-permissions` | Gán permission cho role theo `RolePermissionRequest` |

### Tags và tab khám phá

Tag là nội dung điều khiển tab trên giao diện. `Tags` lưu metadata/tab; `HotelTags` lưu quan hệ nhiều-nhiều tới `KhachSan`. Tham khảo [docs/database-tags.sql](docs/database-tags.sql) để tạo schema.

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/tags/admin` | Danh sách tag gồm tag đang bật/tắt |
| `POST` | `/api/tags/admin` | Tạo tag; slug duy nhất dạng ASCII chữ thường, ví dụ `da-lat` |
| `PUT` | `/api/tags/admin/{tagId}` | Cập nhật tên, slug, mô tả, thứ tự và trạng thái |
| `PUT` | `/api/tags/admin/{tagId}/hotels` | Thay toàn bộ danh sách khách sạn gắn tag |
| `DELETE` | `/api/tags/admin/{tagId}` | Tắt tag mềm; không xóa dữ liệu liên kết |
| `GET` | `/api/tags` | Public: các tag đang bật, theo `SortOrder` |
| `GET` | `/api/tags/{slug}/hotels?page=1&pageSize=10` | Public: khách sạn của tab/tag |

Tạo tag:

```json
{
  "name": "Đà Lạt",
  "slug": "da-lat",
  "description": "Khách sạn tại Đà Lạt",
  "sortOrder": 1,
  "isActive": true
}
```

Gán khách sạn (PUT thay thế toàn bộ liên kết cũ):

```json
{
  "hotelIds": [
    "00000000-0000-0000-0000-000000000001",
    "00000000-0000-0000-0000-000000000002"
  ]
}
```

## Phạm vi cần hoàn thiện

Các endpoint hiện tại chưa bao phủ gán role trực tiếp cho user, dashboard tổng hợp, cấu hình hệ thống, audit log, refund, duyệt Owner/khách sạn theo luồng riêng và moderation review dạng ẩn/khôi phục. Đây là backlog thiết kế, chưa phải endpoint có thể gọi; nên bổ sung authorization theo resource và audit trail trước khi cho Admin can thiệp các nghiệp vụ tài chính.
