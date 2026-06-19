using ForumAPI.DTOs.Admin;

namespace ForumAPI.Services;

public interface IAdminService
{
    Task<AdminDashboardResponse> GetDashboardAsync();
    Task<IEnumerable<AdminNguoiDungResponse>> GetUsersAsync(string? keyword = null);

    Task<IEnumerable<AdminCauHoiResponse>> GetCauHoiAsync(string? keyword = null, int? isDeleted = null);
    Task<bool> DeleteCauHoiAsync(int id);
    Task<bool> RestoreCauHoiAsync(int id);

    Task<IEnumerable<AdminCauTraLoiResponse>> GetCauTraLoiAsync(int? cauHoiId = null, int? isDeleted = null);
    Task<bool> DeleteCauTraLoiAsync(int id);
    Task<bool> RestoreCauTraLoiAsync(int id);

    Task<IEnumerable<AdminBinhLuanResponse>> GetBinhLuanAsync(string? loaiDoiTuong = null, int? isDeleted = null);
    Task<bool> DeleteBinhLuanAsync(int id);
    Task<bool> RestoreBinhLuanAsync(int id);
    Task KhoaNguoiDungAsync(int idNguoiDung, int? currentUserId);

    Task MoKhoaNguoiDungAsync(int idNguoiDung);

    Task CapNhatVaiTroNguoiDungAsync(int idNguoiDung, string vaiTro, int? currentUserId);
}
