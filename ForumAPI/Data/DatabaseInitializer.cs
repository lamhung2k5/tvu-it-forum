using Dapper;

namespace ForumAPI.Data
{
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            // Sử dụng "using" để đảm bảo kết nối SQLite được đóng ngay sau khi chạy xong lệnh
            using var connection = _connectionFactory.CreateConnection();
            
            // Câu lệnh SQL tạo bảng NGUOIDUNG nếu chưa tồn tại
            var sql = @"
                CREATE TABLE IF NOT EXISTS NGUOIDUNG (
                    ID_NguoiDung INTEGER PRIMARY KEY AUTOINCREMENT,
                    HoTen TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    MatKhauHash TEXT NOT NULL,
                    AnhDaiDien TEXT,
                    VaiTro TEXT NOT NULL DEFAULT 'User',
                    TrangThai INTEGER NOT NULL DEFAULT 1,
                    NgayTao TEXT NOT NULL,
                    NgayCapNhat TEXT
                );";

            // Dapper thực thi câu lệnh SQL trực tiếp
            connection.Execute(sql);
        }
    }
}