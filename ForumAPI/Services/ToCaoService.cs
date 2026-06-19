using ForumAPI.DTOs.ToCao;
using ForumAPI.Models;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class ToCaoService : IToCaoService
{
    private const string CAUHOI = "CAUHOI";
    private const string CAUTRALOI = "CAUTRALOI";
    private const string BINHLUAN = "BINHLUAN";

    private readonly IToCaoRepository _toCaoRepository;
    private readonly ICauHoiRepository _cauHoiRepository;
    private readonly ICauTraLoiRepository _cauTraLoiRepository;
    private readonly IBinhLuanRepository _binhLuanRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IThongBaoService _thongBaoService;
    private readonly INguoiDungRepository _nguoiDungRepository;

    public ToCaoService(
        IToCaoRepository toCaoRepository,
        ICauHoiRepository cauHoiRepository,
        ICauTraLoiRepository cauTraLoiRepository,
        IBinhLuanRepository binhLuanRepository,
        IAdminRepository adminRepository,
        IThongBaoService thongBaoService,
        INguoiDungRepository nguoiDungRepository)
    {
        _toCaoRepository = toCaoRepository;
        _cauHoiRepository = cauHoiRepository;
        _cauTraLoiRepository = cauTraLoiRepository;
        _binhLuanRepository = binhLuanRepository;
        _adminRepository = adminRepository;
        _thongBaoService = thongBaoService;
        _nguoiDungRepository = nguoiDungRepository;
    }

    public async Task<int> CreateAsync(int userId, CreateToCaoRequest request)
    {
        await EnsureUserActiveAsync(userId);

        var loaiDoiTuong = NormalizeTargetType(request.LoaiDoiTuong);

        if (request.ID_DoiTuong <= 0)
        {
            throw new ArgumentException("Nội dung tố cáo không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(request.LyDo))
        {
            throw new ArgumentException("Vui lòng chọn lý do tố cáo.");
        }

        var info = await GetTargetInfoAsync(loaiDoiTuong, request.ID_DoiTuong);

        if (info == null)
        {
            throw new InvalidOperationException("Nội dung cần tố cáo không tồn tại hoặc đã bị xóa.");
        }

        if (info.ID_NguoiDung == userId)
        {
            throw new InvalidOperationException("Bạn không thể tố cáo nội dung của chính mình.");
        }

        if (await _toCaoRepository.HasUserReportedAsync(userId, loaiDoiTuong, request.ID_DoiTuong))
        {
            throw new InvalidOperationException("Bạn đã tố cáo nội dung này trước đó.");
        }

        var toCao = new ToCao
        {
            ID_NguoiToCao = userId,
            LoaiDoiTuong = loaiDoiTuong,
            ID_DoiTuong = request.ID_DoiTuong,
            LyDo = request.LyDo.Trim(),
            MoTa = string.IsNullOrWhiteSpace(request.MoTa) ? null : request.MoTa.Trim()
        };

        return await _toCaoRepository.CreateAsync(toCao);
    }

    public async Task<IEnumerable<ToCaoResponse>> GetAllAsync(string? trangThai = null)
    {
        var status = NormalizeStatusOrNull(trangThai);
        return await _toCaoRepository.GetAllAsync(status);
    }

    public async Task<int> CountPendingAsync()
    {
        return await _toCaoRepository.CountPendingAsync();
    }

    public async Task<bool> RejectAsync(int id, int adminId, XuLyToCaoRequest request)
    {
        return await UpdateReportStatusAsync(id, "REJECTED", adminId, request?.GhiChuXuLy);
    }

    public async Task<bool> RemindAsync(int id, int adminId, XuLyToCaoRequest request)
    {
        var report = await GetReportOrThrowAsync(id);
        var note = CleanNote(request?.GhiChuXuLy);
        var success = await UpdateReportStatusAsync(id, "REMINDED", adminId, note);

        if (success && report.ID_NguoiBiToCao.HasValue)
        {
            await _thongBaoService.CreateAsync(
                report.ID_NguoiBiToCao.Value,
                adminId,
                "REPORT_WARNING",
                "Nhắc nhở từ quản trị viên",
                string.IsNullOrWhiteSpace(note)
                    ? "Nội dung của bạn đã được quản trị viên nhắc nhở sau khi có tố cáo từ người dùng khác. Vui lòng kiểm tra và chỉnh sửa nếu cần."
                    : note,
                report.Link
            );
        }

        return success;
    }

    public async Task<bool> ResolveAsync(int id, int adminId, XuLyToCaoRequest request)
    {
        var report = await GetReportOrThrowAsync(id);

        if (report.LoaiDoiTuong == CAUHOI)
        {
            await _adminRepository.AdminDeleteCauHoiAsync(report.ID_DoiTuong);
        }
        else if (report.LoaiDoiTuong == CAUTRALOI)
        {
            await _adminRepository.AdminDeleteCauTraLoiAsync(report.ID_DoiTuong);
        }
        else if (report.LoaiDoiTuong == BINHLUAN)
        {
            await _adminRepository.AdminDeleteBinhLuanAsync(report.ID_DoiTuong);
        }

        var note = CleanNote(request?.GhiChuXuLy);
        var success = await UpdateReportStatusAsync(id, "RESOLVED", adminId, note);

        if (success && report.ID_NguoiBiToCao.HasValue)
        {
            await _thongBaoService.CreateAsync(
                report.ID_NguoiBiToCao.Value,
                adminId,
                "REPORT_DELETE",
                "Nội dung đã bị xử lý",
                string.IsNullOrWhiteSpace(note)
                    ? "Một nội dung của bạn đã bị quản trị viên xóa mềm sau khi xem xét tố cáo."
                    : note,
                report.Link
            );
        }

        return success;
    }

    private async Task<bool> UpdateReportStatusAsync(int id, string status, int adminId, string? note)
    {
        if (id <= 0)
        {
            return false;
        }

        var exists = await _toCaoRepository.GetByIdAsync(id);
        if (exists == null)
        {
            throw new KeyNotFoundException("Không tìm thấy tố cáo cần xử lý.");
        }

        return await _toCaoRepository.UpdateStatusAsync(id, status, adminId, CleanNote(note));
    }

    private async Task<ToCaoResponse> GetReportOrThrowAsync(int id)
    {
        var report = await _toCaoRepository.GetByIdAsync(id);

        if (report == null)
        {
            throw new KeyNotFoundException("Không tìm thấy tố cáo cần xử lý.");
        }

        return report;
    }

    private async Task<ContentOwnerInfo?> GetTargetInfoAsync(string loaiDoiTuong, int doiTuongId)
    {
        return loaiDoiTuong switch
        {
            CAUHOI => await _cauHoiRepository.GetOwnerInfoAsync(doiTuongId),
            CAUTRALOI => await _cauTraLoiRepository.GetOwnerInfoAsync(doiTuongId),
            BINHLUAN => await _binhLuanRepository.GetOwnerInfoAsync(doiTuongId),
            _ => null
        };
    }

    private async Task EnsureUserActiveAsync(int userId)
    {
        if (!await _nguoiDungRepository.IsActiveAsync(userId))
        {
            throw new InvalidOperationException("Tài khoản của bạn đã bị khóa, không thể thực hiện thao tác này.");
        }
    }

    private static string NormalizeTargetType(string value)
    {
        var type = (value ?? string.Empty).Trim().ToUpperInvariant();

        if (type != CAUHOI && type != CAUTRALOI && type != BINHLUAN)
        {
            throw new ArgumentException("Loại nội dung tố cáo chỉ được là CAUHOI, CAUTRALOI hoặc BINHLUAN.");
        }

        return type;
    }

    private static string? NormalizeStatusOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var status = value.Trim().ToUpperInvariant();
        return status switch
        {
            "PENDING" or "REMINDED" or "RESOLVED" or "REJECTED" => status,
            _ => throw new ArgumentException("Trạng thái tố cáo không hợp lệ.")
        };
    }

    private static string? CleanNote(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
