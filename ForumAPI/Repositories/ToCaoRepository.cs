using Dapper;
using ForumAPI.Data;
using ForumAPI.DTOs.ToCao;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public class ToCaoRepository : IToCaoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ToCaoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(ToCao toCao)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO TOCAO (ID_NguoiToCao, LoaiDoiTuong, ID_DoiTuong, LyDo, MoTa, TrangThai, NgayTao)
            VALUES (@ID_NguoiToCao, @LoaiDoiTuong, @ID_DoiTuong, @LyDo, @MoTa, 'PENDING', CURRENT_TIMESTAMP);
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, toCao);
    }

    public async Task<bool> HasUserReportedAsync(int userId, string loaiDoiTuong, int doiTuongId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT COUNT(1)
            FROM TOCAO
            WHERE ID_NguoiToCao = @UserId
              AND LoaiDoiTuong = @LoaiDoiTuong
              AND ID_DoiTuong = @DoiTuongId;";

        var count = await connection.ExecuteScalarAsync<int>(sql, new
        {
            UserId = userId,
            LoaiDoiTuong = loaiDoiTuong,
            DoiTuongId = doiTuongId
        });

        return count > 0;
    }

    public async Task<IEnumerable<ToCaoResponse>> GetAllAsync(string? trangThai = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = BuildReportSelectSql(@"
            WHERE (@TrangThai IS NULL OR tc.TrangThai = @TrangThai)
            ORDER BY
                CASE tc.TrangThai
                    WHEN 'PENDING' THEN 0
                    WHEN 'REMINDED' THEN 1
                    WHEN 'RESOLVED' THEN 2
                    ELSE 3
                END,
                tc.ID_ToCao DESC;");

        return await connection.QueryAsync<ToCaoResponse>(sql, new
        {
            TrangThai = string.IsNullOrWhiteSpace(trangThai) ? null : trangThai.Trim().ToUpperInvariant()
        });
    }

    public async Task<int> CountPendingAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT COUNT(1)
            FROM TOCAO
            WHERE TrangThai = 'PENDING';";

        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<ToCaoResponse?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = BuildReportSelectSql(@"
            WHERE tc.ID_ToCao = @Id;");

        return await connection.QueryFirstOrDefaultAsync<ToCaoResponse>(sql, new { Id = id });
    }

    public async Task<bool> UpdateStatusAsync(int id, string trangThai, int adminId, string? ghiChuXuLy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE TOCAO
            SET TrangThai = @TrangThai,
                ID_AdminXuLy = @AdminId,
                GhiChuXuLy = @GhiChuXuLy,
                NgayXuLy = CURRENT_TIMESTAMP
            WHERE ID_ToCao = @Id;";

        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            TrangThai = trangThai,
            AdminId = adminId,
            GhiChuXuLy = ghiChuXuLy
        });

        return rows > 0;
    }

    private static string BuildReportSelectSql(string whereAndOrder)
    {
        return @$"
            SELECT
                tc.ID_ToCao,
                tc.ID_NguoiToCao,
                nguoiToCao.HoTen AS NguoiToCao,
                tc.LoaiDoiTuong,
                tc.ID_DoiTuong,
                tc.LyDo,
                tc.MoTa,
                tc.TrangThai,
                tc.ID_AdminXuLy,
                admin.HoTen AS AdminXuLy,
                tc.GhiChuXuLy,
                tc.NgayTao,
                tc.NgayXuLy,
                CASE
                    WHEN tc.LoaiDoiTuong = 'CAUHOI' THEN q.ID_NguoiDung
                    WHEN tc.LoaiDoiTuong = 'CAUTRALOI' THEN a.ID_NguoiDung
                    WHEN tc.LoaiDoiTuong = 'BINHLUAN' THEN bl.ID_NguoiDung
                END AS ID_NguoiBiToCao,
                nguoiBiToCao.HoTen AS NguoiBiToCao,
                CASE
                    WHEN tc.LoaiDoiTuong = 'CAUHOI' THEN q.TieuDe
                    WHEN tc.LoaiDoiTuong = 'CAUTRALOI' THEN a.NoiDung
                    WHEN tc.LoaiDoiTuong = 'BINHLUAN' THEN bl.NoiDung
                END AS NoiDungBiToCao,
                CASE
                    WHEN tc.LoaiDoiTuong = 'CAUHOI' THEN '/questions/' || q.ID_CauHoi
                    WHEN tc.LoaiDoiTuong = 'CAUTRALOI' THEN '/questions/' || a.ID_CauHoi
                    WHEN tc.LoaiDoiTuong = 'BINHLUAN' AND bl.LoaiDoiTuong = 'CAUHOI' THEN '/questions/' || bl.ID_DoiTuong
                    WHEN tc.LoaiDoiTuong = 'BINHLUAN' AND bl.LoaiDoiTuong = 'CAUTRALOI' THEN '/questions/' || aq.ID_CauHoi
                END AS Link,
                COALESCE(
                    CASE
                        WHEN tc.LoaiDoiTuong = 'CAUHOI' THEN q.IsDeleted
                        WHEN tc.LoaiDoiTuong = 'CAUTRALOI' THEN a.IsDeleted
                        WHEN tc.LoaiDoiTuong = 'BINHLUAN' THEN bl.IsDeleted
                    END,
                    1
                ) AS IsDeleted
            FROM TOCAO tc
            JOIN NGUOIDUNG nguoiToCao ON tc.ID_NguoiToCao = nguoiToCao.ID_NguoiDung
            LEFT JOIN NGUOIDUNG admin ON tc.ID_AdminXuLy = admin.ID_NguoiDung
            LEFT JOIN CAUHOI q ON tc.LoaiDoiTuong = 'CAUHOI' AND tc.ID_DoiTuong = q.ID_CauHoi
            LEFT JOIN CAUTRALOI a ON tc.LoaiDoiTuong = 'CAUTRALOI' AND tc.ID_DoiTuong = a.ID_CauTraLoi
            LEFT JOIN BINHLUAN bl ON tc.LoaiDoiTuong = 'BINHLUAN' AND tc.ID_DoiTuong = bl.ID_BinhLuan
            LEFT JOIN CAUTRALOI aq ON bl.LoaiDoiTuong = 'CAUTRALOI' AND bl.ID_DoiTuong = aq.ID_CauTraLoi
            LEFT JOIN NGUOIDUNG nguoiBiToCao ON nguoiBiToCao.ID_NguoiDung = CASE
                WHEN tc.LoaiDoiTuong = 'CAUHOI' THEN q.ID_NguoiDung
                WHEN tc.LoaiDoiTuong = 'CAUTRALOI' THEN a.ID_NguoiDung
                WHEN tc.LoaiDoiTuong = 'BINHLUAN' THEN bl.ID_NguoiDung
            END
            {whereAndOrder}";
    }
}
