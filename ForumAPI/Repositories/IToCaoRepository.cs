using ForumAPI.DTOs.ToCao;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public interface IToCaoRepository
{
    Task<int> CreateAsync(ToCao toCao);
    Task<bool> HasUserReportedAsync(int userId, string loaiDoiTuong, int doiTuongId);
    Task<IEnumerable<ToCaoResponse>> GetAllAsync(string? trangThai = null);
    Task<int> CountPendingAsync();
    Task<ToCaoResponse?> GetByIdAsync(int id);
    Task<bool> UpdateStatusAsync(int id, string trangThai, int adminId, string? ghiChuXuLy);
}
