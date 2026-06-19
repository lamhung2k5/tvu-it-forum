# Hướng dẫn test Sprint 4 - Bình chọn

## 1. Chạy backend

```bash
cd ForumAPI
dotnet build
dotnet run
```

Mở Swagger và bấm **Authorize** với token đăng nhập:

```text
Bearer <token>
```

## 2. Chuẩn bị dữ liệu

Cần có ít nhất:

- 1 người dùng đã đăng nhập.
- 1 câu hỏi đang tồn tại.
- 1 câu trả lời đang tồn tại.

Nếu chưa có câu hỏi, gọi:

```http
POST /api/cauhoi
```

Body mẫu:

```json
{
  "idChuyenMuc": 1,
  "tieuDe": "Lỗi khi học Dapper trong .NET",
  "noiDung": "Mình muốn hỏi cách dùng Dapper với SQLite trong .NET."
}
```

Nếu chưa có câu trả lời, gọi:

```http
POST /api/cauhoi/{id}/cautraloi
```

Body mẫu:

```json
{
  "noiDung": "Bạn có thể dùng ExecuteAsync cho INSERT/UPDATE/DELETE và QueryAsync cho SELECT."
}
```

## 3. Test vote câu hỏi

Endpoint:

```http
POST /api/cauhoi/{id}/binhchon
```

Upvote:

```json
{
  "giaTri": 1
}
```

Kết quả mong muốn:

```json
{
  "diemBinhChon": 1,
  "binhChonCuaToi": 1,
  "message": "Bình chọn thành công."
}
```

Bấm lại đúng `giaTri = 1` lần nữa thì hệ thống hủy vote:

```json
{
  "diemBinhChon": 0,
  "binhChonCuaToi": 0,
  "message": "Đã hủy bình chọn."
}
```

Downvote:

```json
{
  "giaTri": -1
}
```

## 4. Test vote câu trả lời

Endpoint:

```http
POST /api/cautraloi/{id}/binhchon
```

Body:

```json
{
  "giaTri": 1
}
```

Kết quả mong muốn tương tự vote câu hỏi.

## 5. Kiểm tra điểm vote khi GET dữ liệu

Gọi:

```http
GET /api/cauhoi
GET /api/cauhoi/{id}
GET /api/cauhoi/{id}/cautraloi
GET /api/cautraloi/{id}
```

Các response cần có trường:

```json
"diemBinhChon": 1
```

## 6. Test lỗi cần có

### Chưa đăng nhập

Không truyền token rồi gọi API vote.

Kết quả mong muốn:

```text
401 Unauthorized
```

### Vote sai giá trị

Body:

```json
{
  "giaTri": 2
}
```

Kết quả mong muốn:

```text
400 Bad Request
```

### Vote nội dung không tồn tại

Gọi:

```http
POST /api/cauhoi/9999/binhchon
POST /api/cautraloi/9999/binhchon
```

Kết quả mong muốn:

```text
404 Not Found
```
