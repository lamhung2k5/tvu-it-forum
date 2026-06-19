using ForumAPI.DTOs.CauTraLoi;
using ForumAPI.Models;

namespace ForumAPI.Repositories;

public interface ICauTraLoiRepository
{
    Task<bool> CauHoiExistsActiveAsync(int cauHoiId);
    Task<int> CreateAsync(CauTraLoi cauTraLoi);
    Task<IEnumerable<CauTraLoiResponse>> GetByCauHoiIdAsync(int cauHoiId);
    Task<CauTraLoiResponse?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, int userId, string noiDung);
    Task<bool> DeleteAsync(int id, int userId);
    Task<bool> AcceptAsync(int id, int userId);
}
