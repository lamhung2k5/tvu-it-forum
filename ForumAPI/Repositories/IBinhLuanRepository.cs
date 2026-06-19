using ForumAPI.DTOs.BinhLuan;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public interface IBinhLuanRepository
{
    Task<bool> ExistsDoiTuongAsync(string loaiDoiTuong, int doiTuongId);
    Task<int> CreateAsync(BinhLuan binhLuan);
    Task<IEnumerable<BinhLuanResponse>> GetByTargetAsync(string loaiDoiTuong, int doiTuongId);
    Task<BinhLuanResponse?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, int userId, string noiDung);
    Task<bool> DeleteAsync(int id, int userId);
}
