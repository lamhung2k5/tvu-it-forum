using ForumAPI.DTOs.CauTraLoi;
using ForumAPI.Models;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class CauTraLoiService : ICauTraLoiService
{
    private readonly ICauTraLoiRepository _cauTraLoiRepository;

    public CauTraLoiService(ICauTraLoiRepository cauTraLoiRepository)
    {
        _cauTraLoiRepository = cauTraLoiRepository;
    }

    public async Task<int> CreateCauTraLoiAsync(int cauHoiId, int userId, CreateCauTraLoiRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new ArgumentException("Nội dung câu trả lời không được để trống.");
        }

        var cauHoiTonTai = await _cauTraLoiRepository.CauHoiExistsActiveAsync(cauHoiId);
        if (!cauHoiTonTai)
        {
            throw new InvalidOperationException("Câu hỏi không tồn tại hoặc đã bị xóa.");
        }

        var cauTraLoi = new CauTraLoi
        {
            ID_CauHoi = cauHoiId,
            ID_NguoiDung = userId,
            NoiDung = request.NoiDung.Trim()
        };

        return await _cauTraLoiRepository.CreateAsync(cauTraLoi);
    }

    public async Task<IEnumerable<CauTraLoiResponse>> GetCauTraLoiByCauHoiIdAsync(int cauHoiId)
    {
        var cauHoiTonTai = await _cauTraLoiRepository.CauHoiExistsActiveAsync(cauHoiId);
        if (!cauHoiTonTai)
        {
            throw new InvalidOperationException("Câu hỏi không tồn tại hoặc đã bị xóa.");
        }

        return await _cauTraLoiRepository.GetByCauHoiIdAsync(cauHoiId);
    }

    public async Task<CauTraLoiResponse?> GetCauTraLoiByIdAsync(int id)
    {
        return await _cauTraLoiRepository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateCauTraLoiAsync(int id, int userId, UpdateCauTraLoiRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new ArgumentException("Nội dung câu trả lời không được để trống.");
        }

        return await _cauTraLoiRepository.UpdateAsync(id, userId, request.NoiDung.Trim());
    }

    public async Task<bool> DeleteCauTraLoiAsync(int id, int userId)
    {
        return await _cauTraLoiRepository.DeleteAsync(id, userId);
    }

    public async Task<bool> AcceptCauTraLoiAsync(int id, int userId)
    {
        return await _cauTraLoiRepository.AcceptAsync(id, userId);
    }

    public async Task<bool> UnacceptAnswerAsync(int id, int userId)
    {
        if (id <= 0)
        {
            throw new Exception("Câu trả lời không hợp lệ.");
        }

        return await _cauTraLoiRepository.UnacceptAsync(id, userId);
    }
}
