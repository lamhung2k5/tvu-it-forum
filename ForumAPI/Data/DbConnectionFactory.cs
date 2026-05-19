 using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ForumAPI.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            // Đọc chuỗi kết nối từ appsettings.json, nếu không thấy sẽ báo lỗi ngay lập tức
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public IDbConnection CreateConnection()
        {
            // Trả về một kết nối SQLite mới
            return new SqliteConnection(_connectionString);
        }
    }
}