using Dapper;
using ForumAPI.Data;
using ForumAPI.DTOs.CauTraLoi;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public class CauTraLoiRepository : ICauTraLoiRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CauTraLoiRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> CauHoiExistsActiveAsync(int cauHoiId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT COUNT(1)
            FROM CAUHOI
            WHERE ID_CauHoi = @CauHoiId
              AND IsDeleted = 0;";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { CauHoiId = cauHoiId });
        return count > 0;
    }

    public async Task<int> CreateAsync(CauTraLoi cauTraLoi)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO CAUTRALOI (ID_CauHoi, ID_NguoiDung, NoiDung, DaChapNhan, IsDeleted, NgayTao)
            VALUES (@ID_CauHoi, @ID_NguoiDung, @NoiDung, 0, 0, CURRENT_TIMESTAMP);
            SELECT last_insert_rowid();";

        return await connection.ExecuteScalarAsync<int>(sql, cauTraLoi);
    }

    public async Task<IEnumerable<CauTraLoiResponse>> GetByCauHoiIdAsync(int cauHoiId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                ctl.ID_CauTraLoi,
                ctl.ID_CauHoi,
                ctl.ID_NguoiDung,
                nd.HoTen,
                nd.AnhDaiDien,
                ctl.NoiDung,
                ctl.DaChapNhan,
                COALESCE(SUM(bc.GiaTri), 0) AS DiemBinhChon,
                (SELECT COUNT(1)
                 FROM BINHLUAN bl
                 WHERE bl.LoaiDoiTuong = 'CAUTRALOI'
                   AND bl.ID_DoiTuong = ctl.ID_CauTraLoi
                   AND bl.IsDeleted = 0) AS SoBinhLuan,
                ctl.NgayTao,
                ctl.NgayCapNhat
            FROM CAUTRALOI ctl
            JOIN NGUOIDUNG nd ON ctl.ID_NguoiDung = nd.ID_NguoiDung
            LEFT JOIN BINHCHON bc ON bc.LoaiDoiTuong = 'CAUTRALOI' AND bc.ID_DoiTuong = ctl.ID_CauTraLoi
            WHERE ctl.ID_CauHoi = @CauHoiId
              AND ctl.IsDeleted = 0
            GROUP BY ctl.ID_CauTraLoi, ctl.ID_CauHoi, ctl.ID_NguoiDung, nd.HoTen, nd.AnhDaiDien, ctl.NoiDung, ctl.DaChapNhan, ctl.NgayTao, ctl.NgayCapNhat
            ORDER BY ctl.DaChapNhan DESC, DiemBinhChon DESC, ctl.ID_CauTraLoi ASC;";

        return await connection.QueryAsync<CauTraLoiResponse>(sql, new { CauHoiId = cauHoiId });
    }

    public async Task<CauTraLoiResponse?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT
                ctl.ID_CauTraLoi,
                ctl.ID_CauHoi,
                ctl.ID_NguoiDung,
                nd.HoTen,
                nd.AnhDaiDien,
                ctl.NoiDung,
                ctl.DaChapNhan,
                COALESCE(SUM(bc.GiaTri), 0) AS DiemBinhChon,
                (SELECT COUNT(1)
                 FROM BINHLUAN bl
                 WHERE bl.LoaiDoiTuong = 'CAUTRALOI'
                   AND bl.ID_DoiTuong = ctl.ID_CauTraLoi
                   AND bl.IsDeleted = 0) AS SoBinhLuan,
                ctl.NgayTao,
                ctl.NgayCapNhat
            FROM CAUTRALOI ctl
            JOIN NGUOIDUNG nd ON ctl.ID_NguoiDung = nd.ID_NguoiDung
            LEFT JOIN BINHCHON bc ON bc.LoaiDoiTuong = 'CAUTRALOI' AND bc.ID_DoiTuong = ctl.ID_CauTraLoi
            WHERE ctl.ID_CauTraLoi = @Id
              AND ctl.IsDeleted = 0
            GROUP BY ctl.ID_CauTraLoi, ctl.ID_CauHoi, ctl.ID_NguoiDung, nd.HoTen, nd.AnhDaiDien, ctl.NoiDung, ctl.DaChapNhan, ctl.NgayTao, ctl.NgayCapNhat;";

        return await connection.QueryFirstOrDefaultAsync<CauTraLoiResponse>(sql, new { Id = id });
    }

    public async Task<bool> UpdateAsync(int id, int userId, string noiDung)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE CAUTRALOI
            SET NoiDung = @NoiDung,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauTraLoi = @Id
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
            UPDATE CAUTRALOI
            SET IsDeleted = 1,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauTraLoi = @Id
              AND ID_NguoiDung = @UserId
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }

    public async Task<bool> AcceptAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var checkSql = @"
            SELECT
                ctl.ID_CauTraLoi,
                ctl.ID_CauHoi,
                ch.ID_NguoiDung AS ID_ChuCauHoi
            FROM CAUTRALOI ctl
            JOIN CAUHOI ch ON ctl.ID_CauHoi = ch.ID_CauHoi
            WHERE ctl.ID_CauTraLoi = @Id
              AND ctl.IsDeleted = 0
              AND ch.IsDeleted = 0;";

        var target = await connection.QueryFirstOrDefaultAsync(checkSql, new { Id = id });

        if (target == null)
        {
            return false;
        }

        int idCauHoi = Convert.ToInt32(target.ID_CauHoi);
        int idChuCauHoi = Convert.ToInt32(target.ID_ChuCauHoi);

        if (idChuCauHoi != userId)
        {
            return false;
        }

        using var transaction = connection.BeginTransaction();

        try
        {
            var clearSql = @"
                UPDATE CAUTRALOI
                SET DaChapNhan = 0,
                    NgayCapNhat = CURRENT_TIMESTAMP
                WHERE ID_CauHoi = @IdCauHoi;";

            var acceptSql = @"
                UPDATE CAUTRALOI
                SET DaChapNhan = 1,
                    NgayCapNhat = CURRENT_TIMESTAMP
                WHERE ID_CauTraLoi = @Id
                  AND IsDeleted = 0;";

            await connection.ExecuteAsync(clearSql, new { IdCauHoi = idCauHoi }, transaction);
            var rowsAffected = await connection.ExecuteAsync(acceptSql, new { Id = id }, transaction);

            transaction.Commit();
            return rowsAffected > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UnacceptAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE CAUTRALOI
            SET DaChapNhan = 0,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_CauTraLoi = @Id
            AND IsDeleted = 0
            AND DaChapNhan = 1
            AND ID_CauHoi IN (
                SELECT ID_CauHoi
                FROM CAUHOI
                WHERE ID_NguoiDung = @UserId
                    AND IsDeleted = 0
            );";

        var rowsAffected = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            UserId = userId
        });

        return rowsAffected > 0;
    }
}
