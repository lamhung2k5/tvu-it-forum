using Dapper;
using ForumAPI.Data;
using ForumAPI.DTOs.Admin;

namespace ForumAPI.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AdminRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<AdminDashboardResponse> GetDashboardAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var totalsSql = @"
            SELECT
                (SELECT COUNT(1) FROM NGUOIDUNG) AS TongNguoiDung,
                (SELECT COUNT(1) FROM CAUHOI WHERE IsDeleted = 0) AS TongCauHoi,
                (SELECT COUNT(1) FROM CAUTRALOI WHERE IsDeleted = 0) AS TongCauTraLoi,
                (SELECT COUNT(1) FROM BINHLUAN WHERE IsDeleted = 0) AS TongBinhLuan,
                (SELECT COUNT(1) FROM CAUHOI WHERE IsDeleted = 1) AS CauHoiDaXoa,
                (SELECT COUNT(1) FROM CAUTRALOI WHERE IsDeleted = 1) AS CauTraLoiDaXoa,
                (SELECT COUNT(1) FROM BINHLUAN WHERE IsDeleted = 1) AS BinhLuanDaXoa;";

        var dashboard = await connection.QuerySingleAsync<AdminDashboardResponse>(totalsSql);

        var contentTypeSql = @"
            SELECT 'Câu hỏi' AS Ten, COUNT(1) AS GiaTri FROM CAUHOI WHERE IsDeleted = 0
            UNION ALL
            SELECT 'Câu trả lời' AS Ten, COUNT(1) AS GiaTri FROM CAUTRALOI WHERE IsDeleted = 0
            UNION ALL
            SELECT 'Bình luận' AS Ten, COUNT(1) AS GiaTri FROM BINHLUAN WHERE IsDeleted = 0;";

        var statusSql = @"
            SELECT 'Đang hiển thị' AS Ten,
                   ((SELECT COUNT(1) FROM CAUHOI WHERE IsDeleted = 0) +
                    (SELECT COUNT(1) FROM CAUTRALOI WHERE IsDeleted = 0) +
                    (SELECT COUNT(1) FROM BINHLUAN WHERE IsDeleted = 0)) AS GiaTri
            UNION ALL
            SELECT 'Đã xóa mềm' AS Ten,
                   ((SELECT COUNT(1) FROM CAUHOI WHERE IsDeleted = 1) +
                    (SELECT COUNT(1) FROM CAUTRALOI WHERE IsDeleted = 1) +
                    (SELECT COUNT(1) FROM BINHLUAN WHERE IsDeleted = 1)) AS GiaTri;";

        var topTagsSql = @"
            SELECT t.TenThe AS Ten, COUNT(cht.ID_CauHoi) AS GiaTri
            FROM THE t
            JOIN CauHoi_The cht ON t.ID_The = cht.ID_The
            JOIN CAUHOI ch ON cht.ID_CauHoi = ch.ID_CauHoi
            WHERE ch.IsDeleted = 0
              AND t.TrangThai = 1
            GROUP BY t.ID_The, t.TenThe
            ORDER BY GiaTri DESC, t.TenThe ASC
            LIMIT 6;";

        var questionsByCategorySql = @"
            SELECT cm.TenChuyenMuc AS Ten, COUNT(ch.ID_CauHoi) AS GiaTri
            FROM CHUYENMUC cm
            LEFT JOIN CAUHOI ch
              ON cm.ID_ChuyenMuc = ch.ID_ChuyenMuc
             AND ch.IsDeleted = 0
            WHERE cm.TrangThai = 1
            GROUP BY cm.ID_ChuyenMuc, cm.TenChuyenMuc
            ORDER BY GiaTri DESC, cm.TenChuyenMuc ASC;";

        dashboard.NoiDungTheoLoai = await connection.QueryAsync<AdminChartItemResponse>(contentTypeSql);
        dashboard.TrangThaiNoiDung = await connection.QueryAsync<AdminChartItemResponse>(statusSql);
        dashboard.TopTags = await connection.QueryAsync<AdminChartItemResponse>(topTagsSql);
        dashboard.CauHoiTheoChuyenMuc = await connection.QueryAsync<AdminChartItemResponse>(questionsByCategorySql);

        return dashboard;
    }

    public async Task<IEnumerable<AdminNguoiDungResponse>> GetUsersAsync(string? keyword = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                ID_NguoiDung,
                HoTen,
                Email,
                AnhDaiDien,
                VaiTro,
                TrangThai,
                NgayTao,
                NgayCapNhat
            FROM NGUOIDUNG
            WHERE 1 = 1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += @"
              AND (HoTen LIKE @Keyword OR Email LIKE @Keyword)";
            parameters.Add("Keyword", $"%{keyword.Trim()}%");
        }

        sql += @"
            ORDER BY ID_NguoiDung DESC;";

        return await connection.QueryAsync<AdminNguoiDungResponse>(sql, parameters);
    }

    public async Task<IEnumerable<AdminCauHoiResponse>> GetCauHoiAsync(string? keyword = null, int? isDeleted = null)
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
                c.TrangThai,
                c.LuotXem,
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
                   AND t.TrangThai = 1) AS Tags,
                c.IsDeleted,
                c.NgayTao,
                c.NgayCapNhat
            FROM CAUHOI c
            JOIN CHUYENMUC cm ON c.ID_ChuyenMuc = cm.ID_ChuyenMuc
            JOIN NGUOIDUNG nd ON c.ID_NguoiDung = nd.ID_NguoiDung
            WHERE 1 = 1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += @"
              AND (c.TieuDe LIKE @Keyword OR c.NoiDung LIKE @Keyword)";
            parameters.Add("Keyword", $"%{keyword.Trim()}%");
        }

        if (isDeleted.HasValue && (isDeleted.Value == 0 || isDeleted.Value == 1))
        {
            sql += @"
              AND c.IsDeleted = @IsDeleted";
            parameters.Add("IsDeleted", isDeleted.Value);
        }

        sql += @"
            ORDER BY c.IsDeleted ASC, c.ID_CauHoi DESC;";

        return await connection.QueryAsync<AdminCauHoiResponse>(sql, parameters);
    }

    public async Task<bool> AdminDeleteCauHoiAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE CAUHOI
            SET IsDeleted = 1,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauHoi = @Id
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> AdminRestoreCauHoiAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE CAUHOI
            SET IsDeleted = 0,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauHoi = @Id
              AND IsDeleted = 1;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<AdminCauTraLoiResponse>> GetCauTraLoiAsync(int? cauHoiId = null, int? isDeleted = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                ctl.ID_CauTraLoi,
                ctl.ID_CauHoi,
                ch.TieuDe AS TieuDeCauHoi,
                ctl.ID_NguoiDung,
                nd.HoTen,
                nd.AnhDaiDien,
                ctl.NoiDung,
                ctl.DaChapNhan,
                (SELECT COALESCE(SUM(bc.GiaTri), 0)
                 FROM BINHCHON bc
                 WHERE bc.LoaiDoiTuong = 'CAUTRALOI'
                   AND bc.ID_DoiTuong = ctl.ID_CauTraLoi) AS DiemBinhChon,
                (SELECT COUNT(1)
                 FROM BINHLUAN bl
                 WHERE bl.LoaiDoiTuong = 'CAUTRALOI'
                   AND bl.ID_DoiTuong = ctl.ID_CauTraLoi
                   AND bl.IsDeleted = 0) AS SoBinhLuan,
                ctl.IsDeleted,
                ctl.NgayTao,
                ctl.NgayCapNhat
            FROM CAUTRALOI ctl
            JOIN CAUHOI ch ON ctl.ID_CauHoi = ch.ID_CauHoi
            JOIN NGUOIDUNG nd ON ctl.ID_NguoiDung = nd.ID_NguoiDung
            WHERE 1 = 1";

        var parameters = new DynamicParameters();

        if (cauHoiId.HasValue && cauHoiId.Value > 0)
        {
            sql += @"
              AND ctl.ID_CauHoi = @CauHoiId";
            parameters.Add("CauHoiId", cauHoiId.Value);
        }

        if (isDeleted.HasValue && (isDeleted.Value == 0 || isDeleted.Value == 1))
        {
            sql += @"
              AND ctl.IsDeleted = @IsDeleted";
            parameters.Add("IsDeleted", isDeleted.Value);
        }

        sql += @"
            ORDER BY ctl.IsDeleted ASC, ctl.ID_CauTraLoi DESC;";

        return await connection.QueryAsync<AdminCauTraLoiResponse>(sql, parameters);
    }

    public async Task<bool> AdminDeleteCauTraLoiAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE CAUTRALOI
            SET IsDeleted = 1,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauTraLoi = @Id
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> AdminRestoreCauTraLoiAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE CAUTRALOI
            SET IsDeleted = 0,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauTraLoi = @Id
              AND IsDeleted = 1;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<AdminBinhLuanResponse>> GetBinhLuanAsync(string? loaiDoiTuong = null, int? isDeleted = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                bl.ID_BinhLuan,
                bl.ID_NguoiDung,
                nd.HoTen,
                nd.AnhDaiDien,
                bl.LoaiDoiTuong,
                bl.ID_DoiTuong,
                CASE
                    WHEN bl.LoaiDoiTuong = 'CAUHOI' THEN (
                        SELECT ch.TieuDe
                        FROM CAUHOI ch
                        WHERE ch.ID_CauHoi = bl.ID_DoiTuong
                    )
                    WHEN bl.LoaiDoiTuong = 'CAUTRALOI' THEN (
                        SELECT ch.TieuDe
                        FROM CAUTRALOI ctl
                        JOIN CAUHOI ch ON ctl.ID_CauHoi = ch.ID_CauHoi
                        WHERE ctl.ID_CauTraLoi = bl.ID_DoiTuong
                    )
                    ELSE NULL
                END AS TieuDeDoiTuong,
                bl.NoiDung,
                bl.IsDeleted,
                bl.NgayTao,
                bl.NgayCapNhat
            FROM BINHLUAN bl
            JOIN NGUOIDUNG nd ON bl.ID_NguoiDung = nd.ID_NguoiDung
            WHERE 1 = 1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(loaiDoiTuong))
        {
            var normalizedType = loaiDoiTuong.Trim().ToUpperInvariant();
            if (normalizedType == "CAUHOI" || normalizedType == "CAUTRALOI")
            {
                sql += @"
                  AND bl.LoaiDoiTuong = @LoaiDoiTuong";
                parameters.Add("LoaiDoiTuong", normalizedType);
            }
        }

        if (isDeleted.HasValue && (isDeleted.Value == 0 || isDeleted.Value == 1))
        {
            sql += @"
              AND bl.IsDeleted = @IsDeleted";
            parameters.Add("IsDeleted", isDeleted.Value);
        }

        sql += @"
            ORDER BY bl.IsDeleted ASC, bl.ID_BinhLuan DESC;";

        return await connection.QueryAsync<AdminBinhLuanResponse>(sql, parameters);
    }

    public async Task<bool> AdminDeleteBinhLuanAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE BINHLUAN
            SET IsDeleted = 1,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_BinhLuan = @Id
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<bool> AdminRestoreBinhLuanAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE BINHLUAN
            SET IsDeleted = 0,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_BinhLuan = @Id
              AND IsDeleted = 1;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
