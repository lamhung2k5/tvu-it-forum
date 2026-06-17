using ForumAPI.DTOs.BinhLuan;
using ForumAPI.Models;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class BinhLuanService : IBinhLuanService
{
    private const string LoaiCauHoi = "CAUHOI";
    private const string LoaiCauTraLoi = "CAUTRALOI";

    private readonly IBinhLuanRepository _binhLuanRepository;

    public BinhLuanService(IBinhLuanRepository binhLuanRepository)
    {
        _binhLuanRepository = binhLuanRepository;
    }

    public async Task<int> CreateBinhLuanCauHoiAsync(int cauHoiId, int userId, CreateBinhLuanRequest request)
    {
        return await CreateAsync(LoaiCauHoi, cauHoiId, userId, request);
    }

    public async Task<int> CreateBinhLuanCauTraLoiAsync(int cauTraLoiId, int userId, CreateBinhLuanRequest request)
    {
        return await CreateAsync(LoaiCauTraLoi, cauTraLoiId, userId, request);
    }

    public async Task<IEnumerable<BinhLuanResponse>> GetBinhLuanCauHoiAsync(int cauHoiId)
    {
        if (!await _binhLuanRepository.ExistsDoiTuongAsync(LoaiCauHoi, cauHoiId))
        {
            throw new InvalidOperationException("Câu hỏi không tồn tại hoặc đã bị xóa.");
        }

        return await _binhLuanRepository.GetByTargetAsync(LoaiCauHoi, cauHoiId);
    }

    public async Task<IEnumerable<BinhLuanResponse>> GetBinhLuanCauTraLoiAsync(int cauTraLoiId)
    {
        if (!await _binhLuanRepository.ExistsDoiTuongAsync(LoaiCauTraLoi, cauTraLoiId))
        {
            throw new InvalidOperationException("Câu trả lời không tồn tại hoặc đã bị xóa.");
        }

        return await _binhLuanRepository.GetByTargetAsync(LoaiCauTraLoi, cauTraLoiId);
    }

    public async Task<BinhLuanResponse?> GetBinhLuanByIdAsync(int id)
    {
        return await _binhLuanRepository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateBinhLuanAsync(int id, int userId, UpdateBinhLuanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new ArgumentException("Nội dung bình luận không được để trống.");
        }

        return await _binhLuanRepository.UpdateAsync(id, userId, request.NoiDung.Trim());
    }

    public async Task<bool> DeleteBinhLuanAsync(int id, int userId)
    {
        return await _binhLuanRepository.DeleteAsync(id, userId);
    }

    private async Task<int> CreateAsync(string loaiDoiTuong, int doiTuongId, int userId, CreateBinhLuanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new ArgumentException("Nội dung bình luận không được để trống.");
        }

        if (!await _binhLuanRepository.ExistsDoiTuongAsync(loaiDoiTuong, doiTuongId))
        {
            var message = loaiDoiTuong == LoaiCauHoi
                ? "Câu hỏi không tồn tại hoặc đã bị xóa."
                : "Câu trả lời không tồn tại hoặc đã bị xóa.";

            throw new InvalidOperationException(message);
        }

        var binhLuan = new BinhLuan
        {
            ID_NguoiDung = userId,
            LoaiDoiTuong = loaiDoiTuong,
            ID_DoiTuong = doiTuongId,
            NoiDung = request.NoiDung.Trim()
        };

        return await _binhLuanRepository.CreateAsync(binhLuan);
    }
}
