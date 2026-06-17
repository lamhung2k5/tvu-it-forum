using ForumAPI.DTOs.BinhLuan;

namespace ForumAPI.Services;

public interface IBinhLuanService
{
    Task<int> CreateBinhLuanCauHoiAsync(int cauHoiId, int userId, CreateBinhLuanRequest request);
    Task<int> CreateBinhLuanCauTraLoiAsync(int cauTraLoiId, int userId, CreateBinhLuanRequest request);
    Task<IEnumerable<BinhLuanResponse>> GetBinhLuanCauHoiAsync(int cauHoiId);
    Task<IEnumerable<BinhLuanResponse>> GetBinhLuanCauTraLoiAsync(int cauTraLoiId);
    Task<BinhLuanResponse?> GetBinhLuanByIdAsync(int id);
    Task<bool> UpdateBinhLuanAsync(int id, int userId, UpdateBinhLuanRequest request);
    Task<bool> DeleteBinhLuanAsync(int id, int userId);
}
