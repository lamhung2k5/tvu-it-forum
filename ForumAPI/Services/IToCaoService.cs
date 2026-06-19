using ForumAPI.DTOs.ToCao;

namespace ForumAPI.Services;

public interface IToCaoService
{
    Task<int> CreateAsync(int userId, CreateToCaoRequest request);
    Task<IEnumerable<ToCaoResponse>> GetAllAsync(string? trangThai = null);
    Task<int> CountPendingAsync();
    Task<bool> RejectAsync(int id, int adminId, XuLyToCaoRequest request);
    Task<bool> RemindAsync(int id, int adminId, XuLyToCaoRequest request);
    Task<bool> ResolveAsync(int id, int adminId, XuLyToCaoRequest request);
}
