# Admin API

Admin có quyền toàn hệ thống. Route yêu cầu JWT role `Admin`; dữ liệu tài chính hoặc trạng thái quan trọng cần được ghi audit trước khi mở rộng quy trình vận hành.

## Cấu hình

- Base URL mặc định: `http://localhost:5295`
- Header: `Authorization: Bearer {{adminAccessToken}}`
- JSON: `Content-Type: application/json`; tạo banner dùng `multipart/form-data`.
- Schema tag: chạy [database-tags.sql](database-tags.sql). Các cột/feature Owner mới: chạy [owner-portal.sql](owner-portal.sql).

## User, property và booking

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/users/admin?keyword={keyword}&status=0&page=1&pageSize=20` | Tìm tài khoản theo username/tên/email; status `0` hoạt động, `1` khóa |
| `PUT` | `/api/users/admin/{userId}/status` | Khóa/mở khóa bằng body `{ "isDel": 1 }`; Admin không tự khóa mình |
| `POST` | `/api/users/role-permissions` | Gán permission cho role |
| `GET` | `/api/hotels/admin/search` | Lọc property theo keyword, tỉnh/thành, sao, trạng thái, phân trang |
| `PUT` | `/api/hotels/admin/{hotelId}/status` | Cập nhật trạng thái property, gồm duyệt/từ chối/khóa theo enum |
| `POST` | `/api/bookings/admin/search` | Tìm booking toàn hệ thống theo filter trong body |
| `GET` | `/api/payments/admin` | Danh sách payment |
| `PUT` | `/api/payments/admin/{paymentId}/status` | Cập nhật trạng thái payment |

## Nội dung và moderation

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/banners?keyword={keyword}&isActive=0&startRow=0&endRow=50` | Tìm banner |
| `POST` | `/api/banners` | Tạo banner, multipart form |
| `PUT` | `/api/banners/{id}` | Sửa banner |
| `DELETE` | `/api/banners/{id}` | Xóa banner |
| `DELETE` | `/api/reviews/{reviewId}` | Xóa review vi phạm |
| `GET` | `/api/advertising/admin/pending` | Đơn quảng cáo chờ duyệt |
| `PUT` | `/api/advertising/admin/{id}/approve` | Duyệt quảng cáo |
| `GET` | `/api/commissions/admin/pending` | Hoa hồng chờ xử lý |
| `GET` | `/api/finance/admin/withdrawals/pending` | Yêu cầu rút tiền chờ xử lý |
| `PUT` | `/api/finance/admin/withdrawals/{id}` | Duyệt/từ chối payout |

## Tab khám phá / Tag

`Tags` lưu tên/slug/thứ tự/hiển thị; `HotelTags` liên kết nhiều-nhiều với khách sạn. Owner không tự sửa danh sách tag hệ thống.

| Method | Endpoint | Mô tả |
|---|---|---|
| `GET` | `/api/tags/admin` | Danh sách tag, gồm tag bật/tắt |
| `POST` | `/api/tags/admin` | Tạo tag, slug ASCII chữ thường duy nhất |
| `PUT` | `/api/tags/admin/{tagId}` | Sửa tag hoặc trạng thái |
| `PUT` | `/api/tags/admin/{tagId}/hotels` | Thay toàn bộ danh sách hotel IDs được gắn |
| `DELETE` | `/api/tags/admin/{tagId}` | Tắt tag mềm |

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

Gán khách sạn (PUT thay thế toàn bộ liên kết hiện tại):

```json
{
  "hotelIds": [
    "00000000-0000-0000-0000-000000000001",
    "00000000-0000-0000-0000-000000000002"
  ]
}
```

## Phạm vi chưa có API

Chưa có dashboard admin tổng hợp, audit log, refund, luồng duyệt Owner riêng, gán role trực tiếp cho user, hoặc moderation review dạng ẩn/khôi phục. Đây là phần cần đặc tả và triển khai riêng; không nên dùng thao tác xóa payment/booking để thay cho refund hay audit.
