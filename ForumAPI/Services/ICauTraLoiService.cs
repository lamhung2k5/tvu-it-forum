using ForumAPI.DTOs.CauTraLoi;

namespace ForumAPI.Services;

public interface ICauTraLoiService
{
    Task<int> CreateCauTraLoiAsync(int cauHoiId, int userId, CreateCauTraLoiRequest request);
    Task<IEnumerable<CauTraLoiResponse>> GetCauTraLoiByCauHoiIdAsync(int cauHoiId);
    Task<CauTraLoiResponse?> GetCauTraLoiByIdAsync(int id);
    Task<bool> UpdateCauTraLoiAsync(int id, int userId, UpdateCauTraLoiRequest request);
    Task<bool> DeleteCauTraLoiAsync(int id, int userId);
    Task<bool> AcceptCauTraLoiAsync(int id, int userId);
    Task<bool> UnacceptAnswerAsync(int id, int userId);
}
