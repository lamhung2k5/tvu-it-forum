# Hướng dẫn test Sprint 5 - Bình luận + Tìm kiếm

## 1. Chuẩn bị

Chạy backend:

```bash
cd ForumAPI
dotnet build
dotnet run
```

Mở Swagger và đăng nhập để lấy JWT token.

Sau khi có token, bấm **Authorize** và nhập:

```text
Bearer <token>
```

Cần có ít nhất:

- 1 câu hỏi còn tồn tại.
- 1 câu trả lời còn tồn tại nếu muốn test bình luận câu trả lời.

---

## 2. Test bình luận câu hỏi

### Thêm bình luận cho câu hỏi

```http
POST /api/cauhoi/{id}/binhluan
```

Body:

```json
{
  "noiDung": "Mình cũng đang gặp lỗi này khi học Dapper với SQLite."
}
```

Kết quả mong muốn:

```json
{
  "message": "Bình luận câu hỏi thành công!",
  "binhLuanId": 1
}
```

### Lấy bình luận của câu hỏi

```http
GET /api/cauhoi/{id}/binhluan
```

Kết quả mong muốn: trả về danh sách bình luận chưa bị xóa mềm.

---

## 3. Test bình luận câu trả lời

### Thêm bình luận cho câu trả lời

```http
POST /api/cautraloi/{id}/binhluan
```

Body:

```json
{
  "noiDung": "Câu trả lời này khá dễ hiểu, cảm ơn bạn."
}
```

### Lấy bình luận của câu trả lời

```http
GET /api/cautraloi/{id}/binhluan
```

---

## 4. Test sửa bình luận

```http
PUT /api/binhluan/{id}
```

Body:

```json
{
  "noiDung": "Mình chỉnh sửa lại nội dung bình luận."
}
```

Kết quả mong muốn:

```json
{
  "message": "Chỉnh sửa bình luận thành công!"
}
```

Lưu ý: chỉ người tạo bình luận mới sửa được.

---

## 5. Test xóa mềm bình luận

```http
DELETE /api/binhluan/{id}
```

Kết quả mong muốn:

```json
{
  "message": "Đã xóa bình luận thành công!"
}
```

Sau khi xóa, gọi lại danh sách bình luận sẽ không thấy bình luận đó nữa.

---

## 6. Test tìm kiếm và lọc câu hỏi

### Tạo câu hỏi có thẻ

```http
POST /api/cauhoi
```

Body:

```json
{
  "idChuyenMuc": 1,
  "tieuDe": "Lỗi khi học Dapper trong .NET",
  "noiDung": "Mình muốn hỏi cách dùng Dapper với SQLite trong .NET.",
  "the": "dotnet,dapper,sqlite"
}
```

### Tìm kiếm theo keyword

```http
GET /api/cauhoi?keyword=dapper
```

### Lọc theo tag

```http
GET /api/cauhoi?tag=sqlite
```

### Lọc theo chuyên mục

```http
GET /api/cauhoi?idChuyenMuc=1
```

### Kết hợp nhiều điều kiện

```http
GET /api/cauhoi?keyword=dapper&tag=sqlite&idChuyenMuc=1
```

---

## 7. Các lỗi cần test

- Không đăng nhập mà POST bình luận → 401 Unauthorized.
- Bình luận rỗng → 400 Bad Request.
- Bình luận vào câu hỏi/câu trả lời không tồn tại → 404 Not Found.
- User khác sửa/xóa bình luận không phải của mình → thất bại.
