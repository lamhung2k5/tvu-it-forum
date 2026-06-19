using Dapper;
using ForumAPI.Data;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public class BinhChonRepository : IBinhChonRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BinhChonRepository(IDbConnectionFactory connectionFactory)
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
                FROM CAUHOI
                WHERE ID_CauHoi = @DoiTuongId
                  AND IsDeleted = 0;";
        }
        else if (loaiDoiTuong == "CAUTRALOI")
        {
            sql = @"
                SELECT COUNT(1)
                FROM CAUTRALOI ctl
                JOIN CAUHOI ch ON ctl.ID_CauHoi = ch.ID_CauHoi
                WHERE ctl.ID_CauTraLoi = @DoiTuongId
                  AND ctl.IsDeleted = 0
                  AND ch.IsDeleted = 0;";
        }
        else
        {
            return false;
        }

        var count = await connection.ExecuteScalarAsync<int>(sql, new { DoiTuongId = doiTuongId });
        return count > 0;
    }

    public async Task<BinhChon?> GetByUserAndTargetAsync(int userId, string loaiDoiTuong, int doiTuongId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT ID_BinhChon, ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, GiaTri, NgayTao, NgayCapNhat
            FROM BINHCHON
            WHERE ID_NguoiDung = @UserId
              AND LoaiDoiTuong = @LoaiDoiTuong
              AND ID_DoiTuong = @DoiTuongId;";

        return await connection.QueryFirstOrDefaultAsync<BinhChon>(sql, new
        {
            UserId = userId,
            LoaiDoiTuong = loaiDoiTuong,
            DoiTuongId = doiTuongId
        });
    }

    public async Task CreateAsync(BinhChon binhChon)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO BINHCHON (ID_NguoiDung, LoaiDoiTuong, ID_DoiTuong, GiaTri, NgayTao)
            VALUES (@ID_NguoiDung, @LoaiDoiTuong, @ID_DoiTuong, @GiaTri, CURRENT_TIMESTAMP);";

        await connection.ExecuteAsync(sql, binhChon);
    }

    public async Task UpdateAsync(int idBinhChon, int giaTri)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE BINHCHON
            SET GiaTri = @GiaTri,
                NgayCapNhat = CURRENT_TIMESTAMP
            WHERE ID_BinhChon = @IdBinhChon;";

        await connection.ExecuteAsync(sql, new { IdBinhChon = idBinhChon, GiaTri = giaTri });
    }

    public async Task DeleteAsync(int idBinhChon)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            DELETE FROM BINHCHON
            WHERE ID_BinhChon = @IdBinhChon;";

        await connection.ExecuteAsync(sql, new { IdBinhChon = idBinhChon });
    }

    public async Task<int> GetDiemBinhChonAsync(string loaiDoiTuong, int doiTuongId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT COALESCE(SUM(GiaTri), 0)
            FROM BINHCHON
            WHERE LoaiDoiTuong = @LoaiDoiTuong
              AND ID_DoiTuong = @DoiTuongId;";

        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            LoaiDoiTuong = loaiDoiTuong,
            DoiTuongId = doiTuongId
        });
    }
}
