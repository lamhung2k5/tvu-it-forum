# ForumWeb - Frontend TVU IT Forum

Frontend được xây dựng bằng Vue 3 + Vite + Element Plus, gọi API thật từ backend `ForumAPI`.

## Chạy frontend

```bash
cd ForumWeb
npm install
npm run dev
```

Mặc định frontend gọi backend ở:

```text
http://localhost:5182
```

Nếu backend chạy port khác, tạo file `.env` trong thư mục `ForumWeb`:

```text
VITE_API_BASE_URL=http://localhost:<PORT_BACKEND>
```

## Các màn hình đã có

- Đăng ký
- Đăng nhập / đăng xuất
- Danh sách câu hỏi
- Tìm kiếm câu hỏi theo từ khóa
- Lọc câu hỏi theo thẻ và chuyên mục
- Đăng câu hỏi
- Sửa/xóa câu hỏi của chính mình
- Chi tiết câu hỏi
- Trả lời câu hỏi
- Sửa/xóa câu trả lời của chính mình
- Chọn câu trả lời được chấp nhận
- Vote câu hỏi / câu trả lời
- Bình luận câu hỏi / câu trả lời
- Trang Admin quản lý user, câu hỏi, câu trả lời, bình luận

## Lưu ý test Admin

Backend kiểm tra role `Admin` từ JWT. Để test Admin, đăng ký user rồi cập nhật role trong SQLite:

```sql
UPDATE NGUOIDUNG
SET VaiTro = 'Admin'
WHERE Email = 'email_cua_ban@gmail.com';
```

Sau đó đăng nhập lại để lấy token mới.
