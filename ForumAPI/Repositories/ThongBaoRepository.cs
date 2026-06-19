using Dapper;
using ForumAPI.Data;
using ForumAPI.DTOs.ThongBao;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public class ThongBaoRepository : IThongBaoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ThongBaoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(ThongBao thongBao)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO THONGBAO (ID_NguoiNhan, ID_NguoiTao, LoaiThongBao, TieuDe, NoiDung, Link, DaDoc, DaXoa, NgayTao)
            VALUES (@ID_NguoiNhan, @ID_NguoiTao, @LoaiThongBao, @TieuDe, @NoiDung, @Link, 0, 0, CURRENT_TIMESTAMP);
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, thongBao);
    }

    public async Task<IEnumerable<ThongBaoResponse>> GetRecentUnreadAsync(int userId, int limit = 5)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = $@"
            {SelectSql}
            WHERE tb.ID_NguoiNhan = @UserId
              AND tb.DaDoc = 0
              AND tb.DaXoa = 0
            ORDER BY tb.ID_ThongBao DESC
            LIMIT @Limit;";

        return await connection.QueryAsync<ThongBaoResponse>(sql, new
        {
            UserId = userId,
            Limit = Math.Clamp(limit, 1, 20)
        });
    }

    public async Task<IEnumerable<ThongBaoResponse>> GetByUserAsync(
        int userId,
        string? status = null,
        string? category = null,
        string? timeRange = null,
        int page = 1,
        int pageSize = 10
    )
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);
        parameters.Add("Offset", (Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 50));
        parameters.Add("PageSize", Math.Clamp(pageSize, 1, 50));

        var where = BuildWhere(status, category, timeRange, parameters);

        var sql = $@"
            {SelectSql}
            {where}
            ORDER BY tb.ID_ThongBao DESC
            LIMIT @PageSize OFFSET @Offset;";

        return await connection.QueryAsync<ThongBaoResponse>(sql, parameters);
    }

    public async Task<int> CountByUserAsync(int userId, string? status = null, string? category = null, string? timeRange = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        var where = BuildWhere(status, category, timeRange, parameters);

        var sql = $@"
            SELECT COUNT(1)
            FROM THONGBAO tb
            {where};";

        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    public async Task<int> CountUnreadAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT COUNT(1)
            FROM THONGBAO
            WHERE ID_NguoiNhan = @UserId
              AND DaDoc = 0
              AND DaXoa = 0;";

        return await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }

    public async Task<bool> MarkAsReadAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE THONGBAO
            SET DaDoc = 1
            WHERE ID_ThongBao = @Id
              AND ID_NguoiNhan = @UserId
              AND DaXoa = 0;";

        var rows = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rows > 0;
    }

    public async Task<int> MarkAllAsReadAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE THONGBAO
            SET DaDoc = 1
            WHERE ID_NguoiNhan = @UserId
              AND DaDoc = 0
              AND DaXoa = 0;";

        return await connection.ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task<bool> SoftDeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE THONGBAO
            SET DaXoa = 1,
                DaDoc = 1
            WHERE ID_ThongBao = @Id
              AND ID_NguoiNhan = @UserId
              AND DaXoa = 0;";

        var rows = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rows > 0;
    }

    public async Task<int> ClearReadAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE THONGBAO
            SET DaXoa = 1
            WHERE ID_NguoiNhan = @UserId
              AND DaDoc = 1
              AND DaXoa = 0;";

        return await connection.ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task<int> ClearAllAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE THONGBAO
            SET DaXoa = 1,
                DaDoc = 1
            WHERE ID_NguoiNhan = @UserId
              AND DaXoa = 0;";

        return await connection.ExecuteAsync(sql, new { UserId = userId });
    }

    private const string SelectSql = @"
        SELECT
            tb.ID_ThongBao,
            tb.ID_NguoiNhan,
            tb.ID_NguoiTao,
            nd.HoTen AS HoTenNguoiTao,
            tb.LoaiThongBao,
            tb.TieuDe,
            tb.NoiDung,
            tb.Link,
            tb.DaDoc,
            tb.DaXoa,
            tb.NgayTao
        FROM THONGBAO tb
        LEFT JOIN NGUOIDUNG nd ON tb.ID_NguoiTao = nd.ID_NguoiDung";

    private static string BuildWhere(string? status, string? category, string? timeRange, DynamicParameters parameters)
    {
        var where = @"
            WHERE tb.ID_NguoiNhan = @UserId
              AND tb.DaXoa = 0";

        var normalizedStatus = (status ?? "all").Trim().ToLowerInvariant();
        if (normalizedStatus == "unread")
        {
            where += @"
              AND tb.DaDoc = 0";
        }
        else if (normalizedStatus == "read")
        {
            where += @"
              AND tb.DaDoc = 1";
        }

        var normalizedCategory = (category ?? "all").Trim().ToLowerInvariant();
        if (normalizedCategory == "interaction")
        {
            where += @"
              AND tb.LoaiThongBao IN ('ANSWER', 'COMMENT_QUESTION', 'COMMENT_ANSWER')";
        }
        else if (normalizedCategory == "admin")
        {
            where += @"
              AND tb.LoaiThongBao IN ('REPORT_WARNING', 'REPORT_DELETE', 'ACCOUNT_LOCK', 'ACCOUNT_UNLOCK', 'SYSTEM')";
        }

        var normalizedTimeRange = (timeRange ?? "all").Trim().ToLowerInvariant();
        if (normalizedTimeRange == "today")
        {
            where += @"
              AND strftime('%Y-%m-%d', tb.NgayTao) = strftime('%Y-%m-%d', 'now', 'localtime')";
        }
        else if (normalizedTimeRange == "week")
        {
            where += @"
              AND strftime('%Y-%W', tb.NgayTao) = strftime('%Y-%W', 'now', 'localtime')";
        }
        else if (normalizedTimeRange == "month")
        {
            where += @"
              AND strftime('%Y-%m', tb.NgayTao) = strftime('%Y-%m', 'now', 'localtime')";
        }
        else if (normalizedTimeRange == "year")
        {
            where += @"
              AND strftime('%Y', tb.NgayTao) = strftime('%Y', 'now', 'localtime')";
        }

        return where;
    }
}
