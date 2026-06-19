using ForumAPI.DTOs.CauHoi;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public interface ICauHoiRepository
{
    Task<int> CreateAsync(CauHoi cauHoi);
    Task<IEnumerable<CauHoiResponse>> GetAllAsync(string? keyword = null, string? tag = null, int? idChuyenMuc = null);
    Task<CauHoiResponse?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, int userId, int idChuyenMuc, string tieuDe, string noiDung);
    Task<bool> DeleteAsync(int id, int userId);
    Task<bool> ChuyenMucExistsAsync(int idChuyenMuc);
    Task SyncTagsAsync(int cauHoiId, string? theRaw);
}
