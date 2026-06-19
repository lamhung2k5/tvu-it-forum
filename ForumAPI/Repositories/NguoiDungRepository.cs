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

            var normalizedEmail = NormalizeEmail(email);

            var sql = @"
                SELECT *
                FROM NGUOIDUNG
                WHERE lower(trim(Email)) = lower(trim(@Email))
                LIMIT 1;
            ";

            return await connection.QueryFirstOrDefaultAsync<NguoiDung>(
                sql,
                new { Email = normalizedEmail }
            );
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();

            var normalizedEmail = NormalizeEmail(email);

            var sql = @"
                SELECT COUNT(1)
                FROM NGUOIDUNG
                WHERE lower(trim(Email)) = lower(trim(@Email));
            ";

            var count = await connection.ExecuteScalarAsync<int>(
                sql,
                new { Email = normalizedEmail }
            );

            return count > 0;
        }

        public async Task<int> CreateAsync(NguoiDung user)
        {
            using var connection = _connectionFactory.CreateConnection();

            user.HoTen = user.HoTen?.Trim() ?? string.Empty;
            user.Email = NormalizeEmail(user.Email);
            user.NgayTao = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            var sql = @"
                INSERT INTO NGUOIDUNG 
                    (HoTen, Email, MatKhauHash, AnhDaiDien, VaiTro, TrangThai, NgayTao)
                VALUES 
                    (@HoTen, @Email, @MatKhauHash, @AnhDaiDien, @VaiTro, @TrangThai, @NgayTao);

                SELECT last_insert_rowid();
            ";

            return await connection.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task<bool> IsActiveAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT COUNT(1)
                FROM NGUOIDUNG
                WHERE ID_NguoiDung = @UserId
                AND TrangThai = 1;";

            var count = await connection.ExecuteScalarAsync<int>(
                sql,
                new { UserId = userId }
            );

            return count > 0;
        }

        private static string NormalizeEmail(string? email)
        {
            return (email ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}