using Dapper;
using ForumAPI.Data;
using ForumAPI.DTOs.BinhLuan;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public class BinhLuanRepository : IBinhLuanRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BinhLuanRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsDoiTuongAsync(string loaiDoiTuong, int doiTuongId)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql;

        if (loaiDoiTuong == "CAUHOI")
        {
            sql = @"
                SELECT COUNT(1)
                FROM CAUHOI ch
                JOIN NGUOIDUNG nd ON ch.ID_NguoiDung = nd.ID_NguoiDung
                WHERE ch.ID_CauHoi = @DoiTuongId
                  AND ch.IsDeleted = 0
                  AND nd.TrangThai = 1;";
        }
        else if (loaiDoiTuong == "CAUTRALOI")
        {
            sql = @"
                SELECT COUNT(1)
                FROM CAUTRALOI ctl
                JOIN NGUOIDUNG ndCtl ON ctl.ID_NguoiDung = ndCtl.ID_NguoiDung
                JOIN CAUHOI ch ON ctl.ID_CauHoi = ch.ID_CauHoi
                JOIN NGUOIDUNG ndCh ON ch.ID_NguoiDung = ndCh.ID_NguoiDung
                WHERE ctl.ID_CauTraLoi = @DoiTuongId
                  AND ctl.IsDeleted = 0
                  AND ch.IsDeleted = 0
                  AND ndCtl.TrangThai = 1
                  AND ndCh.TrangThai = 1;";
        }
        else
        {
            return false;
        }

        var count = await connection.ExecuteScalarAsync<int>(sql, new { DoiTuongId = doiTuongId });
        return count > 0;
    }

    public async Task<int> CreateAsync(BinhLuan binhLuan)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO BINHLUAN (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, NoiDung, IsDeleted, NgayTao)
            VALUES (@ID_NguoiDung, @LoaiDoiTuong, @ID_DoiTuong, @NoiDung, 0, CURRENT_TIMESTAMP);
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, binhLuan);
    }

    public async Task<IEnumerable<BinhLuanResponse>> GetByTargetAsync(string loaiDoiTuong, int doiTuongId)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql;

        if (loaiDoiTuong == "CAUHOI")
        {
            sql = @"
                SELECT
                    bl.ID_BinhLuan,
                    bl.ID_NguoiDung,
                    nd.HoTen,
                    nd.AnhDaiDien,
                    bl.LoaiDoiTuong,
                    bl.ID_DoiTuong,
                    bl.NoiDung,
                    bl.NgayTao,
                    bl.NgayCapNhat
                FROM BINHLUAN bl
                JOIN NGUOIDUNG nd ON bl.ID_NguoiDung = nd.ID_NguoiDung
                JOIN CAUHOI ch ON bl.ID_DoiTuong = ch.ID_CauHoi
                JOIN NGUOIDUNG ndCh ON ch.ID_NguoiDung = ndCh.ID_NguoiDung
                WHERE bl.LoaiDoiTuong = @LoaiDoiTuong
                  AND bl.ID_DoiTuong = @DoiTuongId
                  AND bl.IsDeleted = 0
                  AND ch.IsDeleted = 0
                  AND nd.TrangThai = 1
                  AND ndCh.TrangThai = 1
                ORDER BY bl.ID_BinhLuan ASC;";
        }
        else if (loaiDoiTuong == "CAUTRALOI")
        {
            sql = @"
                SELECT
                    bl.ID_BinhLuan,
                    bl.ID_NguoiDung,
                    nd.HoTen,
                    nd.AnhDaiDien,
                    bl.LoaiDoiTuong,
                    bl.ID_DoiTuong,
                    bl.NoiDung,
                    bl.NgayTao,
                    bl.NgayCapNhat
                FROM BINHLUAN bl
                JOIN NGUOIDUNG nd ON bl.ID_NguoiDung = nd.ID_NguoiDung
                JOIN CAUTRALOI ctl ON bl.ID_DoiTuong = ctl.ID_CauTraLoi
                JOIN NGUOIDUNG ndCtl ON ctl.ID_NguoiDung = ndCtl.ID_NguoiDung
                JOIN CAUHOI ch ON ctl.ID_CauHoi = ch.ID_CauHoi
                JOIN NGUOIDUNG ndCh ON ch.ID_NguoiDung = ndCh.ID_NguoiDung
                WHERE bl.LoaiDoiTuong = @LoaiDoiTuong
                  AND bl.ID_DoiTuong = @DoiTuongId
                  AND bl.IsDeleted = 0
                  AND ctl.IsDeleted = 0
                  AND ch.IsDeleted = 0
                  AND nd.TrangThai = 1
                  AND ndCtl.TrangThai = 1
                  AND ndCh.TrangThai = 1
                ORDER BY bl.ID_BinhLuan ASC;";
        }
        else
        {
            return Enumerable.Empty<BinhLuanResponse>();
        }

        return await connection.QueryAsync<BinhLuanResponse>(sql, new
        {
            LoaiDoiTuong = loaiDoiTuong,
            DoiTuongId = doiTuongId
        });
    }

    public async Task<BinhLuanResponse?> GetByIdAsync(int id)
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
                bl.NoiDung,
                bl.NgayTao,
                bl.NgayCapNhat
            FROM BINHLUAN bl
            JOIN NGUOIDUNG nd ON bl.ID_NguoiDung = nd.ID_NguoiDung
            WHERE bl.ID_BinhLuan = @Id
              AND bl.IsDeleted = 0
              AND nd.TrangThai = 1;";

        return await connection.QueryFirstOrDefaultAsync<BinhLuanResponse>(sql, new { Id = id });
    }

    public async Task<bool> UpdateAsync(int id, int userId, string noiDung)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE BINHLUAN
            SET NoiDung = @NoiDung,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_BinhLuan = @Id
              AND ID_NguoiDung = @UserId
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            UserId = userId,
            NoiDung = noiDung
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE BINHLUAN
            SET IsDeleted = 1,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_BinhLuan = @Id
              AND ID_NguoiDung = @UserId
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }
}