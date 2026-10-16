# Booking API

Backend API cho hệ thống đặt phòng, gồm các nhóm người dùng `Customer`, `Owner` và `Admin`.

## Phân quyền

API sử dụng phân quyền theo role (RBAC đơn giản), không dùng quyền chi tiết theo resource/action:

- Role hiện được cấp: `Customer`, `Owner`, `Admin`.
- Role được lưu trong `Roles`; liên kết người dùng với role lưu trong `UserRoles`.
- Sau khi đăng nhập, client gửi JWT trong header `Authorization: Bearer <token>`.
- API kiểm tra role bằng `[Authorize(Roles = "Admin")]`, `[Authorize(Roles = "Owner")]` hoặc `[Authorize(Roles = "Customer")]`.
- Thiếu hoặc token không hợp lệ thường trả `401 Unauthorized`; đã đăng nhập nhưng sai role trả `403 Forbidden`.

Ví dụ:

```http
GET /api/users/admin
Authorization: Bearer <JWT>
```

Endpoint yêu cầu `Admin` chỉ nhận JWT có role `Admin`. Không có API gán permission cho role; `POST /api/users/role-permissions` đã được gỡ bỏ.

Phân quyền theo role không thay thế việc kiểm tra quyền sở hữu dữ liệu. Các chức năng `Owner` vẫn phải giới hạn khách sạn/phòng/booking theo chủ sở hữu hiện tại trong logic API.

## Tài khoản và xác thực

- `POST /api/users/login`: đăng nhập, nhận access token chứa role.
- `POST /api/users/register`: đăng ký `Customer`.
- `POST /api/users/register-owner`: đăng ký `Owner` chờ Admin duyệt.
- `GET /api/users/me`: xem hồ sơ của người dùng trong token; response gồm `fullName`, `roleId`, `roleName`.
- `POST /api/users/refresh-token`: cấp access token mới từ refresh-token cookie.
- Các API quên/đặt lại mật khẩu và đăng nhập Google được mô tả trong tài liệu API bên dưới.

Ví dụ lấy hồ sơ hiện tại:

```http
GET /api/users/me
Authorization: Bearer <JWT>
```

Response mẫu:

```json
{
  "fullName": "Nguyen Van A",
  "roleId": "00000000-0000-0000-0000-000000000001",
  "roleName": "Customer"
}
```

## Tài liệu API

- [API Customer và endpoint public](docs/api.customer.md)
- [API Owner](docs/api.owner.md)
- [API Admin](docs/api.admin.md)

Tài liệu Swagger được bật trong môi trường Development. Có thể dùng tài liệu theo role ở trên để xem endpoint, request, response và yêu cầu đăng nhập cụ thể.

## Cập nhật schema khi bỏ phân quyền chi tiết

Code không còn truy cập các bảng quyền chi tiết `RolePermissions`, `Permissions`, `Resources`. `Roles` và `UserRoles` vẫn cần được giữ lại để đăng nhập và kiểm tra role.

Nếu database đang có các bảng quyền chi tiết, hãy sao lưu và xác nhận không còn ứng dụng/tiến trình khác phụ thuộc, sau đó chủ động chạy script:

```text
docs/drop-role-permission-tables.sql
```

Script chỉ drop `RolePermissions`, `Permissions`, `Resources`; không được tự chạy khi khởi động ứng dụng. Các bảng tài khoản, `Roles` và `UserRoles` không bị thay đổi.
