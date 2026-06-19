# Hướng dẫn test Sprint 3 - Câu Trả Lời

## 1. Đăng nhập để lấy token

Endpoint:

```http
POST /api/auth/login
```

Body mẫu:

```json
{
  "email": "user@example.com",
  "password": "12345678"
}
```

Copy token trả về và bấm **Authorize** trên Swagger theo dạng:

```text
Bearer <token>
```

## 2. Thêm câu trả lời cho câu hỏi

Endpoint:

```http
POST /api/cauhoi/{id}/cautraloi
```

Body mẫu:

```json
{
  "noiDung": "Theo mình, bạn nên kiểm tra lại cấu hình JWT và connection string trước."
}
```

Kết quả mong muốn: trả về `CauTraLoiId` mới.

## 3. Xem danh sách câu trả lời của câu hỏi

Endpoint:

```http
GET /api/cauhoi/{id}/cautraloi
```

Kết quả mong muốn: danh sách câu trả lời chưa bị xóa mềm.

## 4. Chỉnh sửa câu trả lời

Endpoint:

```http
PUT /api/cautraloi/{id}
```

Body mẫu:

```json
{
  "noiDung": "Nội dung câu trả lời đã được chỉnh sửa."
}
```

Lưu ý: chỉ người tạo câu trả lời mới sửa được.

## 5. Xóa mềm câu trả lời

Endpoint:

```http
DELETE /api/cautraloi/{id}
```

Lưu ý: chỉ người tạo câu trả lời mới xóa được. API không xóa vật lý dữ liệu mà cập nhật `IsDeleted = 1`.

## 6. Chọn câu trả lời được chấp nhận

Endpoint:

```http
PATCH /api/cautraloi/{id}/chapnhan
```

Lưu ý: chỉ người đăng câu hỏi mới được chọn câu trả lời được chấp nhận. Mỗi câu hỏi chỉ có một câu trả lời được chấp nhận.
