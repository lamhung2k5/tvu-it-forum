using ForumAPI.DTOs.ThongBao;

namespace ForumAPI.Services;

public interface IThongBaoService
{
    Task<int> CreateAsync(int nguoiNhanId, int? nguoiTaoId, string loaiThongBao, string tieuDe, string noiDung, string? link = null);
    Task<IEnumerable<ThongBaoResponse>> GetRecentUnreadAsync(int userId, int limit = 5);
    Task<IEnumerable<ThongBaoResponse>> GetMyNotificationsAsync(int userId, string? status = null, string? category = null, string? timeRange = null, int page = 1, int pageSize = 10);
    Task<int> CountMyNotificationsAsync(int userId, string? status = null, string? category = null, string? timeRange = null);
    Task<int> CountUnreadAsync(int userId);
    Task<bool> MarkAsReadAsync(int id, int userId);
    Task<int> MarkAllAsReadAsync(int userId);
    Task<bool> SoftDeleteAsync(int id, int userId);
    Task<int> ClearReadAsync(int userId);
    Task<int> ClearAllAsync(int userId);
}
