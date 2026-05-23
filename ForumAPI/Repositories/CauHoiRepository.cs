using Dapper;
using ForumAPI.Data; // Nơi chứa IDbConnectionFactory của bạn
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
        
        // Câu lệnh SQL viết hoa đúng tên bảng CAUHOI và các trường tiếng Việt
        var sql = @"
            INSERT INTO CAUHOI (ID_NguoiDung, ID_ChuyenMuc, TieuDe, NoiDung, TrangThai, LuotXem, NgayTao, IsDeleted)
            VALUES (@ID_NguoiDung, @ID_ChuyenMuc, @TieuDe, @NoiDung, @TrangThai, @LuotXem, CURRENT_TIMESTAMP, 0);
            SELECT last_insert_rowid();"; // Câu lệnh SQLite để lấy ID vừa tự động tăng

        // Chạy lệnh và lấy về ID dạng số nguyên
        var id = await connection.QuerySingleAsync<int>(sql, cauHoi);
        return id;
    }
    public async Task<IEnumerable<CauHoiResponse>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        // Nối bảng CAUHOI (c) với bảng CHUYENMUC (cm)
        // Lấy ra các câu hỏi chưa bị xóa (IsDeleted = 0), sắp xếp mới nhất lên đầu (DESC)
        var sql = @"
            SELECT 
                c.ID_CauHoi, c.TieuDe, c.NoiDung, c.LuotXem, c.NgayTao,
                cm.TenChuyenMuc
            FROM CAUHOI c
            JOIN CHUYENMUC cm ON c.ID_ChuyenMuc = cm.ID_ChuyenMuc
            WHERE c.IsDeleted = 0
            ORDER BY c.ID_CauHoi DESC;";

        return await connection.QueryAsync<CauHoiResponse>(sql);
    }

    public async Task<CauHoiResponse?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT 
                c.ID_CauHoi, c.TieuDe, c.NoiDung, c.LuotXem, c.NgayTao,
                cm.TenChuyenMuc
            FROM CAUHOI c
            JOIN CHUYENMUC cm ON c.ID_ChuyenMuc = cm.ID_ChuyenMuc
            WHERE c.ID_CauHoi = @Id AND c.IsDeleted = 0;";

        // Dùng QueryFirstOrDefaultAsync để lấy đúng 1 dòng đầu tiên tìm thấy
        return await connection.QueryFirstOrDefaultAsync<CauHoiResponse>(sql, new { Id = id });
    }  

    public async Task<bool> UpdateAsync(int id, int userId, int idChuyenMuc, string tieuDe, string noiDung)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // Cú pháp thần thánh: Vừa UPDATE vừa kiểm tra WHERE ID_NguoiDung = @UserId
        // Nếu không phải bài của user này, câu lệnh sẽ không tìm thấy dòng nào để sửa!
        var sql = @"
            UPDATE CAUHOI 
            SET TieuDe = @TieuDe, 
                NoiDung = @NoiDung, 
                ID_ChuyenMuc = @IdChuyenMuc
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

        return rowsAffected > 0; // Trả về true nếu có dòng được sửa
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // Cập nhật IsDeleted = 1 thay vì xóa thật. Vẫn phải check ID_NguoiDung!
        var sql = @"
            UPDATE CAUHOI 
            SET IsDeleted = 1 
            WHERE ID_CauHoi = @Id 
              AND ID_NguoiDung = @UserId 
              AND IsDeleted = 0;";

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });

        return rowsAffected > 0;
    }
}