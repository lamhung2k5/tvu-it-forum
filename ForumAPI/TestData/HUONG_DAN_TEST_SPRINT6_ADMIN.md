# Hướng dẫn test Sprint 6 - Admin

Sprint 6 bổ sung nhóm API `/api/admin` để Admin quản lý nội dung trong hệ thống.

## 1. Chạy project

```bash
cd ForumAPI
dotnet build
dotnet run
```

Mở Swagger:

```text
http://localhost:5182/swagger
```

## 2. Tạo tài khoản Admin để test

Đăng ký tài khoản bình thường bằng API:

```http
POST /api/auth/register
```

Ví dụ:

```json
{
  "hoTen": "Admin Test",
  "email": "admin@test.com",
  "password": "12345678"
}
```

Sau đó mở database `forum.db` và chạy SQL:

```sql
UPDATE NGUOIDUNG
SET VaiTro = 'Admin'
WHERE Email = 'admin@test.com';
```

Đăng nhập lại tài khoản Admin:

```http
POST /api/auth/login
```

```json
{
  "email": "admin@test.com",
  "password": "12345678"
}
```

Copy `accessToken`, vào Swagger bấm **Authorize**, nhập:

```text
Bearer <accessToken>
```

## 3. Test phân quyền

### 3.1. User thường không được vào Admin

Đăng nhập bằng tài khoản có `VaiTro = User`, gọi:

```http
GET /api/admin/users
```

Kết quả mong muốn:

```text
403 Forbidden
```

### 3.2. Admin được vào Admin

Đăng nhập bằng tài khoản có `VaiTro = Admin`, gọi:

```http
GET /api/admin/users
```

Kết quả mong muốn:

```text
200 OK
```

## 4. API Sprint 6

### Xem danh sách người dùng

```http
GET /api/admin/users
GET /api/admin/users?keyword=admin
```

### Xem câu hỏi, gồm cả câu hỏi đã xóa mềm

```http
GET /api/admin/cauhoi
GET /api/admin/cauhoi?keyword=dapper
GET /api/admin/cauhoi?isDeleted=1
GET /api/admin/cauhoi?isDeleted=0
```

### Admin xóa mềm câu hỏi

```http
DELETE /api/admin/cauhoi/{id}
```

Sau khi xóa, gọi:

```http
GET /api/cauhoi
```

Câu hỏi đã xóa sẽ không còn hiện ở phía user.

Admin vẫn xem được ở:

```http
GET /api/admin/cauhoi?isDeleted=1
```

### Admin khôi phục câu hỏi

```http
PATCH /api/admin/cauhoi/{id}/khoiphuc
```

Sau khi khôi phục, câu hỏi sẽ hiện lại ở:

```http
GET /api/cauhoi
```

### Xem câu trả lời

```http
GET /api/admin/cautraloi
GET /api/admin/cautraloi?cauHoiId=1
GET /api/admin/cautraloi?isDeleted=1
```

### Admin xóa mềm câu trả lời

```http
DELETE /api/admin/cautraloi/{id}
```

### Admin khôi phục câu trả lời

```http
PATCH /api/admin/cautraloi/{id}/khoiphuc
```

### Xem bình luận

```http
GET /api/admin/binhluan
GET /api/admin/binhluan?loaiDoiTuong=CAUHOI
GET /api/admin/binhluan?loaiDoiTuong=CAUTRALOI
GET /api/admin/binhluan?isDeleted=1
```

### Admin xóa mềm bình luận

```http
DELETE /api/admin/binhluan/{id}
```

### Admin khôi phục bình luận

```http
PATCH /api/admin/binhluan/{id}/khoiphuc
```

## 5. Kịch bản demo gợi ý

1. User A đăng câu hỏi.
2. User B trả lời câu hỏi.
3. User C bình luận dưới câu hỏi hoặc câu trả lời.
4. Admin đăng nhập.
5. Admin xem danh sách người dùng.
6. Admin xem danh sách câu hỏi.
7. Admin xóa mềm câu hỏi vi phạm.
8. Kiểm tra `GET /api/cauhoi` không còn thấy câu hỏi đó.
9. Admin khôi phục câu hỏi.
10. Kiểm tra `GET /api/cauhoi` thấy câu hỏi hiện lại.
11. Làm tương tự với câu trả lời và bình luận.

Nếu chạy được kịch bản này thì Sprint 6 đạt yêu cầu demo backend.
