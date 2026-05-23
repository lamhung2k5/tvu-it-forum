using ForumAPI.DTOs.CauHoi;

namespace ForumAPI.Services;

public interface ICauHoiService
{
    // Hàm nhận DTO từ client và ID của người dùng đang đăng nhập
    Task<int> CreateCauHoiAsync(CreateCauHoiRequest request, int userId);
    Task<IEnumerable<CauHoiResponse>> GetAllCauHoiAsync();
    Task<CauHoiResponse?> GetCauHoiByIdAsync(int id);

    Task<bool> UpdateCauHoiAsync(int id, int userId, UpdateCauHoiRequest request);

    Task<bool> DeleteCauHoiAsync(int id, int userId);
}