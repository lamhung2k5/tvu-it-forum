using ForumAPI.DTOs.BinhLuan;
using ForumAPI.Models;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class BinhLuanService : IBinhLuanService
{
    private const string LoaiCauHoi = "CAUHOI";
    private const string LoaiCauTraLoi = "CAUTRALOI";

    private readonly IBinhLuanRepository _binhLuanRepository;
    private readonly ICauHoiRepository _cauHoiRepository;
    private readonly ICauTraLoiRepository _cauTraLoiRepository;
    private readonly INguoiDungRepository _nguoiDungRepository;
    private readonly IThongBaoService _thongBaoService;

    public BinhLuanService(
        IBinhLuanRepository binhLuanRepository,
        ICauHoiRepository cauHoiRepository,
        ICauTraLoiRepository cauTraLoiRepository,
        INguoiDungRepository nguoiDungRepository,
        IThongBaoService thongBaoService
    )
    {
        _binhLuanRepository = binhLuanRepository;
        _cauHoiRepository = cauHoiRepository;
        _cauTraLoiRepository = cauTraLoiRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _thongBaoService = thongBaoService;
    }

    public async Task<int> CreateBinhLuanCauHoiAsync(int cauHoiId, int userId, CreateBinhLuanRequest request)
    {
        var newId = await CreateAsync(LoaiCauHoi, cauHoiId, userId, request);

        var questionInfo = await _cauHoiRepository.GetOwnerInfoAsync(cauHoiId);
        if (questionInfo != null && questionInfo.ID_NguoiDung != userId)
        {
            await _thongBaoService.CreateAsync(
                questionInfo.ID_NguoiDung,
                userId,
                "COMMENT_QUESTION",
                "Câu hỏi của bạn có bình luận mới",
                $"Có người đã bình luận vào câu hỏi: {questionInfo.TieuDe}",
                $"/questions/{cauHoiId}"
            );
        }

        return newId;
    }

    public async Task<int> CreateBinhLuanCauTraLoiAsync(int cauTraLoiId, int userId, CreateBinhLuanRequest request)
    {
        var newId = await CreateAsync(LoaiCauTraLoi, cauTraLoiId, userId, request);

        var answerInfo = await _cauTraLoiRepository.GetOwnerInfoAsync(cauTraLoiId);
        if (answerInfo != null && answerInfo.ID_NguoiDung != userId)
        {
            await _thongBaoService.CreateAsync(
                answerInfo.ID_NguoiDung,
                userId,
                "COMMENT_ANSWER",
                "Câu trả lời của bạn có bình luận mới",
                "Có người đã bình luận vào câu trả lời của bạn.",
                $"/questions/{answerInfo.ID_CauHoi}"
            );
        }

        return newId;
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
        await EnsureUserActiveAsync(userId);
        if (string.IsNullOrWhiteSpace(request.NoiDung))
        {
            throw new ArgumentException("Nội dung bình luận không được để trống.");
        }

        return await _binhLuanRepository.UpdateAsync(id, userId, request.NoiDung.Trim());
    }

    public async Task<bool> DeleteBinhLuanAsync(int id, int userId)
    {
        await EnsureUserActiveAsync(userId);
        return await _binhLuanRepository.DeleteAsync(id, userId);
    }

    private async Task<int> CreateAsync(string loaiDoiTuong, int doiTuongId, int userId, CreateBinhLuanRequest request)
    {
        await EnsureUserActiveAsync(userId);
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

    private async Task EnsureUserActiveAsync(int userId)
    {
        if (!await _nguoiDungRepository.IsActiveAsync(userId))
        {
            throw new InvalidOperationException("Tài khoản của bạn đã bị khóa, không thể thực hiện thao tác này.");
        }
    }
}
