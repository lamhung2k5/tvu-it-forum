using Dapper;
using ForumAPI.Data;
using ForumAPI.DTOs.CauHoi;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public class CauHoiRepository : ICauHoiRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CauHoiRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(CauHoi cauHoi)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            INSERT INTO CAUHOI (ID_NguoiDung, ID_ChuyenMuc, TieuDe, NoiDung, TrangThai, LuotXem, NgayTao, IsDeleted)
            VALUES (@ID_NguoiDung, @ID_ChuyenMuc, @TieuDe, @NoiDung, @TrangThai, @LuotXem, CURRENT_TIMESTAMP, 0);
            SELECT last_insert_rowid();";

        return await connection.QuerySingleAsync<int>(sql, cauHoi);
    }

    public async Task<IEnumerable<CauHoiResponse>> GetAllAsync(string? keyword = null, string? tag = null, int? idChuyenMuc = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                c.ID_CauHoi,
                c.ID_NguoiDung,
                nd.HoTen,
                c.ID_ChuyenMuc,
                cm.TenChuyenMuc,
                c.TieuDe,
                c.NoiDung,
                c.LuotXem,
                c.NgayTao,
                c.NgayCapNhat,
                (SELECT COALESCE(SUM(bc.GiaTri), 0)
                 FROM BINHCHON bc
                 WHERE bc.LoaiDoiTuong = 'CAUHOI'
                   AND bc.ID_DoiTuong = c.ID_CauHoi) AS DiemBinhChon,
                (SELECT COUNT(1)
                 FROM CAUTRALOI ctl
                 WHERE ctl.ID_CauHoi = c.ID_CauHoi
                   AND ctl.IsDeleted = 0) AS SoCauTraLoi,
                (SELECT COUNT(1)
                 FROM BINHLUAN bl
                 WHERE bl.LoaiDoiTuong = 'CAUHOI'
                   AND bl.ID_DoiTuong = c.ID_CauHoi
                   AND bl.IsDeleted = 0) AS SoBinhLuan,
                (SELECT GROUP_CONCAT(t.TenThe, ',')
                 FROM CauHoi_The cht
                 JOIN THE t ON cht.ID_The = t.ID_The
                 WHERE cht.ID_CauHoi = c.ID_CauHoi
                   AND t.TrangThai = 1) AS Tags
            FROM CAUHOI c
            JOIN CHUYENMUC cm ON c.ID_ChuyenMuc = cm.ID_ChuyenMuc
            JOIN NGUOIDUNG nd ON c.ID_NguoiDung = nd.ID_NguoiDung
            WHERE c.IsDeleted = 0";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += @"
              AND (c.TieuDe LIKE @Keyword OR c.NoiDung LIKE @Keyword)";
            parameters.Add("Keyword", $"%{keyword.Trim()}%");
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            sql += @"
              AND EXISTS (
                  SELECT 1
                  FROM CauHoi_The chtFilter
                  JOIN THE tFilter ON chtFilter.ID_The = tFilter.ID_The
                  WHERE chtFilter.ID_CauHoi = c.ID_CauHoi
                    AND LOWER(tFilter.TenThe) = LOWER(@Tag)
                    AND tFilter.TrangThai = 1
              )";
            parameters.Add("Tag", tag.Trim());
        }

        if (idChuyenMuc.HasValue && idChuyenMuc.Value > 0)
        {
            sql += @"
              AND c.ID_ChuyenMuc = @IdChuyenMuc";
            parameters.Add("IdChuyenMuc", idChuyenMuc.Value);
        }

        sql += @"
            ORDER BY c.ID_CauHoi DESC;";

        return await connection.QueryAsync<CauHoiResponse>(sql, parameters);
    }

    public async Task<CauHoiResponse?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                c.ID_CauHoi,
                c.ID_NguoiDung,
                nd.HoTen,
                c.ID_ChuyenMuc,
                cm.TenChuyenMuc,
                c.TieuDe,
                c.NoiDung,
                c.LuotXem,
                c.NgayTao,
                c.NgayCapNhat,
                (SELECT COALESCE(SUM(bc.GiaTri), 0)
                 FROM BINHCHON bc
                 WHERE bc.LoaiDoiTuong = 'CAUHOI'
                   AND bc.ID_DoiTuong = c.ID_CauHoi) AS DiemBinhChon,
                (SELECT COUNT(1)
                 FROM CAUTRALOI ctl
                 WHERE ctl.ID_CauHoi = c.ID_CauHoi
                   AND ctl.IsDeleted = 0) AS SoCauTraLoi,
                (SELECT COUNT(1)
                 FROM BINHLUAN bl
                 WHERE bl.LoaiDoiTuong = 'CAUHOI'
                   AND bl.ID_DoiTuong = c.ID_CauHoi
                   AND bl.IsDeleted = 0) AS SoBinhLuan,
                (SELECT GROUP_CONCAT(t.TenThe, ',')
                 FROM CauHoi_The cht
                 JOIN THE t ON cht.ID_The = t.ID_The
                 WHERE cht.ID_CauHoi = c.ID_CauHoi
                   AND t.TrangThai = 1) AS Tags
            FROM CAUHOI c
            JOIN CHUYENMUC cm ON c.ID_ChuyenMuc = cm.ID_ChuyenMuc
            JOIN NGUOIDUNG nd ON c.ID_NguoiDung = nd.ID_NguoiDung
            WHERE c.ID_CauHoi = @Id
              AND c.IsDeleted = 0;";

        return await connection.QueryFirstOrDefaultAsync<CauHoiResponse>(sql, new { Id = id });
    }  

    public async Task<bool> IncreaseViewAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE CAUHOI
            SET LuotXem = COALESCE(LuotXem, 0) + 1
            WHERE ID_CauHoi = @Id
            AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(int id, int userId, int idChuyenMuc, string tieuDe, string noiDung)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            UPDATE CAUHOI 
            SET TieuDe = @TieuDe, 
                NoiDung = @NoiDung, 
                ID_ChuyenMuc = @IdChuyenMuc,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauHoi = @Id 
              AND ID_NguoiDung = @UserId 
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new 
        { 
            Id = id, 
            UserId = userId, 
            IdChuyenMuc = idChuyenMuc, 
            TieuDe = tieuDe, 
            NoiDung = noiDung 
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            UPDATE CAUHOI 
            SET IsDeleted = 1,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauHoi = @Id 
              AND ID_NguoiDung = @UserId 
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }

    public async Task<bool> ChuyenMucExistsAsync(int idChuyenMuc)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT COUNT(1)
            FROM CHUYENMUC
            WHERE ID_ChuyenMuc = @IdChuyenMuc
              AND TrangThai = 1;";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { IdChuyenMuc = idChuyenMuc });
        return count > 0;
    }

    public async Task SyncTagsAsync(int cauHoiId, string? theRaw)
    {
        if (theRaw == null)
        {
            return;
        }

        var tags = theRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .Take(10)
            .ToList();

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var deleteSql = @"
                DELETE FROM CauHoi_The
                WHERE ID_CauHoi = @CauHoiId;";
            await connection.ExecuteAsync(deleteSql, new { CauHoiId = cauHoiId }, transaction);

            foreach (var tag in tags)
            {
                var insertTagSql = @"
                    INSERT OR IGNORE INTO THE (TenThe, TrangThai)
                    VALUES (@TenThe, 1);";
                await connection.ExecuteAsync(insertTagSql, new { TenThe = tag }, transaction);

                var getTagIdSql = @"
                    SELECT ID_The
                    FROM THE
                    WHERE TenThe = @TenThe;";
                var tagId = await connection.ExecuteScalarAsync<int>(getTagIdSql, new { TenThe = tag }, transaction);

                var insertLinkSql = @"
                    INSERT OR IGNORE INTO CauHoi_The (ID_CauHoi, ID_The)
                    VALUES (@CauHoiId, @TagId);";
                await connection.ExecuteAsync(insertLinkSql, new { CauHoiId = cauHoiId, TagId = tagId }, transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
