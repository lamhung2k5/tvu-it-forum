using Dapper;
using ForumAPI.Data;
using ForumAPI.DTOs.User;

namespace ForumAPI.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserProfileRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UserProfileResponse?> GetProfileAsync(int userId, bool includeEmail)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = $@"
            SELECT
                nd.ID_NguoiDung,
                nd.HoTen,
                {(includeEmail ? "nd.Email" : "NULL")} AS Email,
                nd.AnhDaiDien,
                nd.VaiTro,
                nd.TrangThai,
                nd.NgayTao,
                (SELECT COUNT(1)
                 FROM CAUHOI ch
                 WHERE ch.ID_NguoiDung = nd.ID_NguoiDung
                   AND ch.IsDeleted = 0) AS SoCauHoi,
                (SELECT COUNT(1)
                 FROM CAUTRALOI ctl
                 WHERE ctl.ID_NguoiDung = nd.ID_NguoiDung
                   AND ctl.IsDeleted = 0) AS SoCauTraLoi,
                (SELECT COUNT(1)
                 FROM BINHLUAN bl
                 WHERE bl.ID_NguoiDung = nd.ID_NguoiDung
                   AND bl.IsDeleted = 0) AS SoBinhLuan,
                (SELECT COUNT(1)
                 FROM CAUTRALOI ctl
                 WHERE ctl.ID_NguoiDung = nd.ID_NguoiDung
                   AND ctl.DaChapNhan = 1
                   AND ctl.IsDeleted = 0) AS SoCauTraLoiDuocChapNhan,
                (SELECT COALESCE(SUM(score.Diem), 0)
                 FROM (
                     SELECT COALESCE(SUM(bc.GiaTri), 0) AS Diem
                     FROM CAUHOI ch
                     LEFT JOIN BINHCHON bc
                       ON bc.LoaiDoiTuong = 'CAUHOI'
                      AND bc.ID_DoiTuong = ch.ID_CauHoi
                     WHERE ch.ID_NguoiDung = nd.ID_NguoiDung
                       AND ch.IsDeleted = 0
                     GROUP BY ch.ID_CauHoi
                     UNION ALL
                     SELECT COALESCE(SUM(bc.GiaTri), 0) AS Diem
                     FROM CAUTRALOI ctl
                     LEFT JOIN BINHCHON bc
                       ON bc.LoaiDoiTuong = 'CAUTRALOI'
                      AND bc.ID_DoiTuong = ctl.ID_CauTraLoi
                     WHERE ctl.ID_NguoiDung = nd.ID_NguoiDung
                       AND ctl.IsDeleted = 0
                     GROUP BY ctl.ID_CauTraLoi
                 ) score) AS TongDiemBinhChon
            FROM NGUOIDUNG nd
            WHERE nd.ID_NguoiDung = @UserId
              AND nd.TrangThai = 1
            LIMIT 1;";

        return await connection.QueryFirstOrDefaultAsync<UserProfileResponse>(sql, new { UserId = userId });
    }

    public async Task<bool> UpdateProfileAsync(int userId, string hoTen)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE NGUOIDUNG
            SET HoTen = @HoTen,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_NguoiDung = @UserId
              AND TrangThai = 1;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = userId, HoTen = hoTen });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<UserQuestionResponse>> GetMyQuestionsAsync(int userId, int limit = 0, bool includeDeleted = true)
    {
        using var connection = _connectionFactory.CreateConnection();

        var limitClause = limit > 0 ? " LIMIT @Limit" : string.Empty;
        var deletedFilter = includeDeleted ? string.Empty : " AND c.IsDeleted = 0";
        var orderClause = includeDeleted
            ? "ORDER BY c.IsDeleted ASC, c.ID_CauHoi DESC"
            : "ORDER BY c.ID_CauHoi DESC";

        var sql = $@"
            SELECT
                c.ID_CauHoi,
                c.ID_NguoiDung,
                nd.HoTen,
                c.ID_ChuyenMuc,
                cm.TenChuyenMuc,
                c.TieuDe,
                c.NoiDung,
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
            WHERE c.ID_NguoiDung = @UserId
            {deletedFilter}
            {orderClause}{limitClause};";

        return await connection.QueryAsync<UserQuestionResponse>(sql, new
        {
            UserId = userId,
            Limit = limit
        });
    }

    public async Task<IEnumerable<UserQuestionResponse>> GetPublicQuestionsAsync(int userId)
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
            WHERE c.ID_NguoiDung = @UserId
            AND c.IsDeleted = 0
            AND nd.TrangThai = 1
            ORDER BY c.ID_CauHoi DESC;";

        return await connection.QueryAsync<UserQuestionResponse>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<UserAnswerResponse>> GetPublicAnswersAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                ctl.ID_CauTraLoi,
                ctl.ID_CauHoi,
                ch.TieuDe AS TieuDeCauHoi,
                ctl.ID_NguoiDung,
                nd.HoTen,
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
            WHERE ctl.ID_NguoiDung = @UserId
            AND ctl.IsDeleted = 0
            AND ch.IsDeleted = 0
            AND nd.TrangThai = 1
            ORDER BY ctl.ID_CauTraLoi DESC;";

        return await connection.QueryAsync<UserAnswerResponse>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<UserAnswerResponse>> GetMyAnswersAsync(int userId, int limit = 0, bool includeDeleted = true)
    {
        using var connection = _connectionFactory.CreateConnection();

        var limitClause = limit > 0 ? " LIMIT @Limit" : string.Empty;
        var deletedFilter = includeDeleted ? string.Empty : " AND ctl.IsDeleted = 0 AND ch.IsDeleted = 0";
        var orderClause = includeDeleted
            ? "ORDER BY ctl.IsDeleted ASC, ctl.ID_CauTraLoi DESC"
            : "ORDER BY ctl.ID_CauTraLoi DESC";

        var sql = $@"
            SELECT
                ctl.ID_CauTraLoi,
                ctl.ID_CauHoi,
                ch.TieuDe AS TieuDeCauHoi,
                ctl.ID_NguoiDung,
                nd.HoTen,
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
            WHERE ctl.ID_NguoiDung = @UserId
            {deletedFilter}
            {orderClause}{limitClause};";

        return await connection.QueryAsync<UserAnswerResponse>(sql, new
        {
            UserId = userId,
            Limit = limit
        });
    }

    public async Task<IEnumerable<UserCommentResponse>> GetMyCommentsAsync(int userId, int limit = 0)
    {
        using var connection = _connectionFactory.CreateConnection();
        var limitClause = limit > 0 ? " LIMIT @Limit" : string.Empty;

        var sql = $@"
            SELECT
                bl.ID_BinhLuan,
                bl.ID_NguoiDung,
                nd.HoTen,
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
            WHERE bl.ID_NguoiDung = @UserId
            ORDER BY bl.IsDeleted ASC, bl.ID_BinhLuan DESC{limitClause};";

        return await connection.QueryAsync<UserCommentResponse>(sql, new { UserId = userId, Limit = limit });
    }
}
