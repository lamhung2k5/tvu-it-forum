using ForumAPI.DTOs.CauHoi;
using ForumAPI.Models;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class CauHoiService : ICauHoiService
{
    private readonly ICauHoiRepository _cauHoiRepository;

    public CauHoiService(ICauHoiRepository cauHoiRepository)
    {
        _cauHoiRepository = cauHoiRepository;
    }

    public async Task<int> CreateCauHoiAsync(CreateCauHoiRequest request, int userId)
    {
        if (request.ID_ChuyenMuc <= 0 || !await _cauHoiRepository.ChuyenMucExistsAsync(request.ID_ChuyenMuc))
        {
            throw new ArgumentException("Chuyên mục không tồn tại. Vui lòng chọn ID chuyên mục hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(request.TieuDe) || string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new ArgumentException("Tiêu đề và nội dung câu hỏi không được để trống.");
        }

        var cauHoi = new CauHoi
        {
            ID_NguoiDung = userId,
            ID_ChuyenMuc = request.ID_ChuyenMuc,
            TieuDe = request.TieuDe.Trim(),
            NoiDung = request.NoiDung.Trim()
        };

        var newId = await _cauHoiRepository.CreateAsync(cauHoi);
        await _cauHoiRepository.SyncTagsAsync(newId, request.The);

        return newId;
    }

    public async Task<IEnumerable<CauHoiResponse>> GetAllCauHoiAsync(string? keyword = null, string? tag = null, int? idChuyenMuc = null)
    {
        return await _cauHoiRepository.GetAllAsync(keyword, tag, idChuyenMuc);
    }

    public async Task<CauHoiResponse?> GetCauHoiByIdAsync(int id, bool tangLuotXem = false)
    {
        if (tangLuotXem)
        {
            await _cauHoiRepository.IncreaseViewAsync(id);
        }

        return await _cauHoiRepository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateCauHoiAsync(int id, int userId, UpdateCauHoiRequest request)
    {
        if (request.ID_ChuyenMuc <= 0 || !await _cauHoiRepository.ChuyenMucExistsAsync(request.ID_ChuyenMuc))
        {
            throw new ArgumentException("Chuyên mục không tồn tại. Vui lòng chọn ID chuyên mục hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(request.TieuDe) || string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new ArgumentException("Tiêu đề và nội dung câu hỏi không được để trống.");
        }

        var isSuccess = await _cauHoiRepository.UpdateAsync(
            id, 
            userId, 
            request.ID_ChuyenMuc, 
            request.TieuDe.Trim(), 
            request.NoiDung.Trim()
        );

        if (isSuccess && request.The != null)
        {
            await _cauHoiRepository.SyncTagsAsync(id, request.The);
        }

        return isSuccess;
    }

    public async Task<bool> DeleteCauHoiAsync(int id, int userId)
    {
        return await _cauHoiRepository.DeleteAsync(id, userId);
    }
}
