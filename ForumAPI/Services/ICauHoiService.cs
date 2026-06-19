using ForumAPI.DTOs.CauHoi;

namespace ForumAPI.Services;

public interface ICauHoiService
{
    Task<int> CreateCauHoiAsync(CreateCauHoiRequest request, int userId);
    Task<IEnumerable<CauHoiResponse>> GetAllCauHoiAsync(string? keyword = null, string? tag = null, int? idChuyenMuc = null);
    Task<CauHoiResponse?> GetCauHoiByIdAsync(int id);
    Task<bool> UpdateCauHoiAsync(int id, int userId, UpdateCauHoiRequest request);
    Task<bool> DeleteCauHoiAsync(int id, int userId);
}
