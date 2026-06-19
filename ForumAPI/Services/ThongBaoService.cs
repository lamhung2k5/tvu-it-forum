using ForumAPI.DTOs.ThongBao;
using ForumAPI.Models;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class ThongBaoService : IThongBaoService
{
    private readonly IThongBaoRepository _thongBaoRepository;

    public ThongBaoService(IThongBaoRepository thongBaoRepository)
    {
        _thongBaoRepository = thongBaoRepository;
    }

    public async Task<int> CreateAsync(int nguoiNhanId, int? nguoiTaoId, string loaiThongBao, string tieuDe, string noiDung, string? link = null)
    {
        if (nguoiNhanId <= 0)
        {
            return 0;
        }

        var thongBao = new ThongBao
        {
            ID_NguoiNhan = nguoiNhanId,
            ID_NguoiTao = nguoiTaoId,
            LoaiThongBao = NormalizeType(loaiThongBao),
            TieuDe = string.IsNullOrWhiteSpace(tieuDe) ? "Thông báo mới" : tieuDe.Trim(),
            NoiDung = string.IsNullOrWhiteSpace(noiDung) ? "Bạn có một thông báo mới." : noiDung.Trim(),
            Link = string.IsNullOrWhiteSpace(link) ? null : link.Trim()
        };

        return await _thongBaoRepository.CreateAsync(thongBao);
    }

    public async Task<IEnumerable<ThongBaoResponse>> GetRecentUnreadAsync(int userId, int limit = 5)
    {
        return await _thongBaoRepository.GetRecentUnreadAsync(userId, limit);
    }

    public async Task<IEnumerable<ThongBaoResponse>> GetMyNotificationsAsync(int userId, string? status = null, string? category = null, string? timeRange = null, int page = 1, int pageSize = 10)
    {
        return await _thongBaoRepository.GetByUserAsync(
            userId,
            NormalizeStatus(status),
            NormalizeCategory(category),
            NormalizeTimeRange(timeRange),
            page,
            pageSize
        );
    }

    public async Task<int> CountMyNotificationsAsync(int userId, string? status = null, string? category = null, string? timeRange = null)
    {
        return await _thongBaoRepository.CountByUserAsync(
            userId,
            NormalizeStatus(status),
            NormalizeCategory(category),
            NormalizeTimeRange(timeRange)
        );
    }

    public async Task<int> CountUnreadAsync(int userId)
    {
        return await _thongBaoRepository.CountUnreadAsync(userId);
    }

    public async Task<bool> MarkAsReadAsync(int id, int userId)
    {
        return await _thongBaoRepository.MarkAsReadAsync(id, userId);
    }

    public async Task<int> MarkAllAsReadAsync(int userId)
    {
        return await _thongBaoRepository.MarkAllAsReadAsync(userId);
    }

    public async Task<bool> SoftDeleteAsync(int id, int userId)
    {
        return await _thongBaoRepository.SoftDeleteAsync(id, userId);
    }

    public async Task<int> ClearReadAsync(int userId)
    {
        return await _thongBaoRepository.ClearReadAsync(userId);
    }

    public async Task<int> ClearAllAsync(int userId)
    {
        return await _thongBaoRepository.ClearAllAsync(userId);
    }

    private static string NormalizeType(string value)
    {
        var type = (value ?? string.Empty).Trim().ToUpperInvariant();
        return string.IsNullOrWhiteSpace(type) ? "SYSTEM" : type;
    }

    private static string NormalizeStatus(string? value)
    {
        var status = (value ?? "all").Trim().ToLowerInvariant();
        return status is "all" or "unread" or "read" ? status : "all";
    }

    private static string NormalizeCategory(string? value)
    {
        var category = (value ?? "all").Trim().ToLowerInvariant();
        return category is "all" or "interaction" or "admin" ? category : "all";
    }

    private static string NormalizeTimeRange(string? value)
    {
        var timeRange = (value ?? "all").Trim().ToLowerInvariant();
        return timeRange is "all" or "today" or "week" or "month" or "year" ? timeRange : "all";
    }
}

