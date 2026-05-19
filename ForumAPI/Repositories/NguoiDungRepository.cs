using Dapper;
using ForumAPI.Data;
using ForumAPI.Models;

namespace ForumAPI.Repositories
{
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public NguoiDungRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<NguoiDung?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM NGUOIDUNG WHERE Email = @Email LIMIT 1;";
            
            // QueryFirstOrDefaultAsync sẽ trả về null nếu không tìm thấy user
            return await connection.QueryFirstOrDefaultAsync<NguoiDung>(sql, new { Email = email });
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(1) FROM NGUOIDUNG WHERE Email = @Email;";
            
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        public async Task<int> CreateAsync(NguoiDung user)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Gán thời gian hiện tại chuẩn ISO cho NgayTao
            user.NgayTao = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            var sql = @"
                INSERT INTO NGUOIDUNG (HoTen, Email, MatKhauHash, AnhDaiDien, VaiTro, TrangThai, NgayTao)
                VALUES (@HoTen, @Email, @MatKhauHash, @AnhDaiDien, @VaiTro, @TrangThai, @NgayTao);
                
                -- Hàm của SQLite để lấy lại ID tự tăng vừa được tạo
                SELECT last_insert_rowid();";

            // ExecuteScalarAsync sẽ thực thi chuỗi INSERT và trả về kết quả của SELECT last_insert_rowid()
            return await connection.ExecuteScalarAsync<int>(sql, user);
        }
    }
}