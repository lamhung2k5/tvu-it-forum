using ForumAPI.DTOs.Admin;

namespace ForumAPI.Repositories;

public interface IAdminRepository
{
    Task<AdminDashboardResponse> GetDashboardAsync();
    Task<IEnumerable<AdminNguoiDungResponse>> GetUsersAsync(string? keyword = null);

    Task<IEnumerable<AdminCauHoiResponse>> GetCauHoiAsync(string? keyword = null, int? isDeleted = null);
    Task<bool> AdminDeleteCauHoiAsync(int id);
    Task<bool> AdminRestoreCauHoiAsync(int id);

    Task<IEnumerable<AdminCauTraLoiResponse>> GetCauTraLoiAsync(int? cauHoiId = null, int? isDeleted = null);
    Task<bool> AdminDeleteCauTraLoiAsync(int id);
    Task<bool> AdminRestoreCauTraLoiAsync(int id);

    Task<IEnumerable<AdminBinhLuanResponse>> GetBinhLuanAsync(string? loaiDoiTuong = null, int? isDeleted = null);
    Task<bool> AdminDeleteBinhLuanAsync(int id);
    Task<bool> AdminRestoreBinhLuanAsync(int id);
}
