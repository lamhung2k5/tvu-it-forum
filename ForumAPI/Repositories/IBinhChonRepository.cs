using ForumAPI.Models;

namespace ForumAPI.Repositories;

public interface IBinhChonRepository
{
    Task<bool> ExistsDoiTuongAsync(string loaiDoiTuong, int doiTuongId);
    Task<BinhChon?> GetByUserAndTargetAsync(int userId, string loaiDoiTuong, int doiTuongId);
    Task CreateAsync(BinhChon binhChon);
    Task UpdateAsync(int idBinhChon, int giaTri);
    Task DeleteAsync(int idBinhChon);
    Task<int> GetDiemBinhChonAsync(string loaiDoiTuong, int doiTuongId);
}
