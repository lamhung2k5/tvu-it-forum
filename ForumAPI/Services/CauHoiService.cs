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
        // Chuyển đổi (Map) từ DTO sang Model thực tế để lưu vào Database
        var cauHoi = new CauHoi
        {
            ID_NguoiDung = userId,
            ID_ChuyenMuc = request.ID_ChuyenMuc,
            TieuDe = request.TieuDe,
            NoiDung = request.NoiDung
            // Các trường như NgayTao, LuotXem, TrangThai... đã có giá trị mặc định bên Model nên không cần gán thêm
        };

        // Gọi Repository để thực thi câu lệnh SQL
        return await _cauHoiRepository.CreateAsync(cauHoi);
    }

    public async Task<IEnumerable<CauHoiResponse>> GetAllCauHoiAsync()
    {
        return await _cauHoiRepository.GetAllAsync();
    }

    public async Task<CauHoiResponse?> GetCauHoiByIdAsync(int id)
    {
        return await _cauHoiRepository.GetByIdAsync(id);
    } 

    public async Task<bool> UpdateCauHoiAsync(int id, int userId, UpdateCauHoiRequest request)
    {
        return await _cauHoiRepository.UpdateAsync(
            id, 
            userId, 
            request.ID_ChuyenMuc, 
            request.TieuDe, 
            request.NoiDung
        );
    }

    public async Task<bool> DeleteCauHoiAsync(int id, int userId)
    {
        return await _cauHoiRepository.DeleteAsync(id, userId);
    }

}