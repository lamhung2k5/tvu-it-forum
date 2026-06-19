using ForumAPI.DTOs.Admin;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }


    public async Task<AdminDashboardResponse> GetDashboardAsync()
    {
        return await _adminRepository.GetDashboardAsync();
    }

    public async Task<IEnumerable<AdminNguoiDungResponse>> GetUsersAsync(string? keyword = null)
    {
        return await _adminRepository.GetUsersAsync(keyword);
    }

    public async Task<IEnumerable<AdminCauHoiResponse>> GetCauHoiAsync(string? keyword = null, int? isDeleted = null)
    {
        return await _adminRepository.GetCauHoiAsync(keyword, NormalizeIsDeleted(isDeleted));
    }

    public async Task<bool> DeleteCauHoiAsync(int id)
    {
        return await _adminRepository.AdminDeleteCauHoiAsync(id);
    }

    public async Task<bool> RestoreCauHoiAsync(int id)
    {
        return await _adminRepository.AdminRestoreCauHoiAsync(id);
    }

    public async Task<IEnumerable<AdminCauTraLoiResponse>> GetCauTraLoiAsync(int? cauHoiId = null, int? isDeleted = null)
    {
        return await _adminRepository.GetCauTraLoiAsync(cauHoiId, NormalizeIsDeleted(isDeleted));
    }

    public async Task<bool> DeleteCauTraLoiAsync(int id)
    {
        return await _adminRepository.AdminDeleteCauTraLoiAsync(id);
    }

    public async Task<bool> RestoreCauTraLoiAsync(int id)
    {
        return await _adminRepository.AdminRestoreCauTraLoiAsync(id);
    }

    public async Task<IEnumerable<AdminBinhLuanResponse>> GetBinhLuanAsync(string? loaiDoiTuong = null, int? isDeleted = null)
    {
        var normalizedType = string.IsNullOrWhiteSpace(loaiDoiTuong)
            ? null
            : loaiDoiTuong.Trim().ToUpperInvariant();

        if (normalizedType != null && normalizedType != "CAUHOI" && normalizedType != "CAUTRALOI")
        {
            throw new ArgumentException("Loại đối tượng chỉ được là CAUHOI hoặc CAUTRALOI.");
        }

        return await _adminRepository.GetBinhLuanAsync(normalizedType, NormalizeIsDeleted(isDeleted));
    }

    public async Task<bool> DeleteBinhLuanAsync(int id)
    {
        return await _adminRepository.AdminDeleteBinhLuanAsync(id);
    }

    public async Task<bool> RestoreBinhLuanAsync(int id)
    {
        return await _adminRepository.AdminRestoreBinhLuanAsync(id);
    }

    private static int? NormalizeIsDeleted(int? isDeleted)
    {
        if (!isDeleted.HasValue)
        {
            return null;
        }

        return isDeleted.Value == 1 ? 1 : 0;
    }
    public async Task KhoaNguoiDungAsync(int idNguoiDung, int? currentUserId)
    {
        if (currentUserId.HasValue && currentUserId.Value == idNguoiDung)
        {
            throw new InvalidOperationException("Không thể tự khóa tài khoản của chính mình.");
        }

        var laAdminDangHoatDong = await _adminRepository.LaAdminDangHoatDongAsync(idNguoiDung);

        if (laAdminDangHoatDong)
        {
            var soAdminDangHoatDong = await _adminRepository.DemAdminDangHoatDongAsync();

            if (soAdminDangHoatDong <= 1)
            {
                throw new InvalidOperationException("Không thể khóa admin đang hoạt động cuối cùng.");
            }
        }

        var success = await _adminRepository.CapNhatTrangThaiNguoiDungAsync(idNguoiDung, 0);

        if (!success)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng cần khóa.");
        }
    }

    public async Task MoKhoaNguoiDungAsync(int idNguoiDung)
    {
        var success = await _adminRepository.CapNhatTrangThaiNguoiDungAsync(idNguoiDung, 1);

        if (!success)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng cần mở khóa.");
        }
    }

    public async Task CapNhatVaiTroNguoiDungAsync(int idNguoiDung, string vaiTro, int? currentUserId)
    {
        vaiTro = (vaiTro ?? string.Empty).Trim();

        if (vaiTro.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            vaiTro = "Admin";
        }
        else if (vaiTro.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            vaiTro = "User";
        }
        else
        {
            throw new ArgumentException("Vai trò không hợp lệ. Chỉ chấp nhận Admin hoặc User.");
        }

        if (currentUserId.HasValue && currentUserId.Value == idNguoiDung && vaiTro == "User")
        {
            throw new InvalidOperationException("Không thể tự hạ quyền tài khoản của chính mình.");
        }

        if (vaiTro == "User")
        {
            var laAdminDangHoatDong = await _adminRepository.LaAdminDangHoatDongAsync(idNguoiDung);

            if (laAdminDangHoatDong)
            {
                var soAdminDangHoatDong = await _adminRepository.DemAdminDangHoatDongAsync();

                if (soAdminDangHoatDong <= 1)
                {
                    throw new InvalidOperationException("Không thể hạ quyền admin đang hoạt động cuối cùng.");
                }
            }
        }

        var success = await _adminRepository.CapNhatVaiTroNguoiDungAsync(idNguoiDung, vaiTro);

        if (!success)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng cần cập nhật vai trò.");
        }
    }
}
