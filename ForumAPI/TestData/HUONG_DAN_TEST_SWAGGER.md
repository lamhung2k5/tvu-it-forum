# 📘 HƯỚNG DẪN KIỂM THỬ API TRONG SWAGGER UI

## 🚀 Bước 1: Khởi động API
```bash
cd ForumAPI
dotnet run
```

Sau khi chạy, truy cập: **http://localhost:5182/swagger**

---

## 📋 Bước 2: Test API Đăng ký (Register)

### 2.1. Mở endpoint `/api/auth/register`
1. Trong Swagger UI, tìm section **Auth**
2. Click vào `POST /api/auth/register`
3. Click nút **"Try it out"**

### 2.2. Nhập dữ liệu JSON
Copy và paste JSON sau vào ô **Request body**:

```json
{
  "hoTen": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "password": "MatKhau123@"
}
```

### 2.3. Thực thi
1. Click nút **"Execute"**
2. Xem kết quả ở phần **Response**

### ✅ Kết quả mong đợi:
- **Status Code**: `200 OK`
- **Response Body**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "nguyenvana@example.com",
  "hoTen": "Nguyễn Văn A"
}
```

---

## 🔐 Bước 3: Test API Đăng nhập (Login)

### 3.1. Mở endpoint `/api/auth/login`
1. Click vào `POST /api/auth/login`
2. Click nút **"Try it out"**

### 3.2. Nhập dữ liệu JSON
```json
{
  "email": "nguyenvana@example.com",
  "password": "MatKhau123@"
}
```

### 3.3. Thực thi
Click **"Execute"**

### ✅ Kết quả mong đợi:
- **Status Code**: `200 OK`
- **Response Body**: Giống như khi đăng ký (có token JWT)

---

## 🧪 Bước 4: Test các trường hợp lỗi

### Test 1: Đăng ký với email đã tồn tại
**Endpoint**: `POST /api/auth/register`
```json
{
  "hoTen": "Người Dùng Khác",
  "email": "nguyenvana@example.com",
  "password": "Pass456!"
}
```
**Kết quả mong đợi**: 
- Status: `400 Bad Request`
- Message: "Email đã tồn tại"

---

### Test 2: Đăng nhập sai mật khẩu
**Endpoint**: `POST /api/auth/login`
```json
{
  "email": "nguyenvana@example.com",
  "password": "SaiMatKhau"
}
```
**Kết quả mong đợi**:
- Status: `400 Bad Request`
- Message: "Sai mật khẩu"

---

### Test 3: Đăng nhập với email không tồn tại
**Endpoint**: `POST /api/auth/login`
```json
{
  "email": "khongtontai@example.com",
  "password": "BatKy123"
}
```
**Kết quả mong đợi**:
- Status: `400 Bad Request`
- Message: "Email không tồn tại"

---

## 📊 Bước 5: Kiểm tra Database

Sau khi test, bạn có thể kiểm tra database SQLite:

### Cách 1: Dùng DB Browser for SQLite
1. Tải **DB Browser for SQLite**: https://sqlitebrowser.org/
2. Mở file `ForumAPI/forum.db`
3. Xem bảng `NGUOIDUNG`

### Cách 2: Dùng command line
```bash
cd ForumAPI
sqlite3 forum.db
SELECT * FROM NGUOIDUNG;
.exit
```

---

## 🎯 Các test case quan trọng

| # | Test Case | Endpoint | Expected Result |
|---|-----------|----------|-----------------|
| 1 | Đăng ký thành công | POST /api/auth/register | 200 OK + JWT token |
| 2 | Đăng ký email trùng | POST /api/auth/register | 400 Bad Request |
| 3 | Đăng nhập thành công | POST /api/auth/login | 200 OK + JWT token |
| 4 | Đăng nhập sai password | POST /api/auth/login | 400 Bad Request |
| 5 | Đăng nhập email không tồn tại | POST /api/auth/login | 400 Bad Request |

---

## 💡 Tips

1. **Copy JWT Token**: Sau khi đăng ký/đăng nhập thành công, copy token để dùng cho các API khác (nếu có)
2. **Clear Database**: Nếu muốn test lại từ đầu, xóa file `forum.db` và restart API
3. **Check Console**: Xem terminal đang chạy API để thấy log lỗi (nếu có)

---

## ❓ Troubleshooting

### Lỗi: "Email đã tồn tại" khi test lần đầu
- **Nguyên nhân**: Database đã có dữ liệu từ lần chạy trước
- **Giải pháp**: Đổi email khác hoặc xóa file `forum.db`

### Lỗi: "Connection refused"
- **Nguyên nhân**: API chưa chạy hoặc port sai
- **Giải pháp**: Kiểm tra terminal, đảm bảo API đang chạy ở port 5182

### Lỗi: "Invalid token"
- **Nguyên nhân**: Token hết hạn hoặc sai format
- **Giải pháp**: Đăng nhập lại để lấy token mới
