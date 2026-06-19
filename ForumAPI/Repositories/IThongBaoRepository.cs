using ForumAPI.DTOs.ThongBao;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public interface IThongBaoRepository
{
    Task<int> CreateAsync(ThongBao thongBao);
    Task<IEnumerable<ThongBaoResponse>> GetRecentUnreadAsync(int userId, int limit = 5);
    Task<IEnumerable<ThongBaoResponse>> GetByUserAsync(int userId, string? status = null, string? category = null, string? timeRange = null, int page = 1, int pageSize = 10);
    Task<int> CountByUserAsync(int userId, string? status = null, string? category = null, string? timeRange = null);
    Task<int> CountUnreadAsync(int userId);
    Task<bool> MarkAsReadAsync(int id, int userId);
    Task<int> MarkAllAsReadAsync(int userId);
    Task<bool> SoftDeleteAsync(int id, int userId);
    Task<int> ClearReadAsync(int userId);
    Task<int> ClearAllAsync(int userId);
}
