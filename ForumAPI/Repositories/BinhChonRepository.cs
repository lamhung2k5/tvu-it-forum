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
            SELECT COALESCE(SUM(bc.GiaTri), 0)
            FROM BINHCHON bc
            JOIN NGUOIDUNG nd ON bc.ID_NguoiDung = nd.ID_NguoiDung
            WHERE bc.LoaiDoiTuong = @LoaiDoiTuong
              AND bc.ID_DoiTuong = @DoiTuongId
              AND nd.TrangThai = 1;";

        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            LoaiDoiTuong = loaiDoiTuong,
            DoiTuongId = doiTuongId
        });
    }
}