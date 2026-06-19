using ForumAPI.DTOs.BinhChon;
using ForumAPI.Models;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class BinhChonService : IBinhChonService
{
    private readonly IBinhChonRepository _binhChonRepository;
    private readonly INguoiDungRepository _nguoiDungRepository;

    public BinhChonService(IBinhChonRepository binhChonRepository,INguoiDungRepository nguoiDungRepository
    )
    {
        _binhChonRepository = binhChonRepository;
        _nguoiDungRepository = nguoiDungRepository;
    }

    public async Task<BinhChonResponse> BinhChonCauHoiAsync(int cauHoiId, int userId, BinhChonRequest request)
    {
        return await XuLyBinhChonAsync("CAUHOI", cauHoiId, userId, request);
    }

    public async Task<BinhChonResponse> BinhChonCauTraLoiAsync(int cauTraLoiId, int userId, BinhChonRequest request)
    {
        return await XuLyBinhChonAsync("CAUTRALOI", cauTraLoiId, userId, request);
    }

    private async Task<BinhChonResponse> XuLyBinhChonAsync(string loaiDoiTuong, int doiTuongId, int userId, BinhChonRequest request)
    {
        await EnsureUserActiveAsync(userId);
        if (request.GiaTri != 1 && request.GiaTri != -1)
        {
            throw new ArgumentException("Giá trị bình chọn chỉ được là 1 hoặc -1.");
        }

        var doiTuongTonTai = await _binhChonRepository.ExistsDoiTuongAsync(loaiDoiTuong, doiTuongId);
        if (!doiTuongTonTai)
        {
            throw new InvalidOperationException("Nội dung cần bình chọn không tồn tại hoặc đã bị xóa.");
        }

        var binhChonHienTai = await _binhChonRepository.GetByUserAndTargetAsync(userId, loaiDoiTuong, doiTuongId);
        var binhChonCuaToi = request.GiaTri;
        var message = "Bình chọn thành công.";

        if (binhChonHienTai == null)
        {
            var binhChonMoi = new BinhChon
            {
                ID_NguoiDung = userId,
                LoaiDoiTuong = loaiDoiTuong,
                ID_DoiTuong = doiTuongId,
                GiaTri = request.GiaTri
            };

            await _binhChonRepository.CreateAsync(binhChonMoi);
        }
        else if (binhChonHienTai.GiaTri == request.GiaTri)
        {
            await _binhChonRepository.DeleteAsync(binhChonHienTai.ID_BinhChon);
            binhChonCuaToi = 0;
            message = "Đã hủy bình chọn.";
        }
        else
        {
            await _binhChonRepository.UpdateAsync(binhChonHienTai.ID_BinhChon, request.GiaTri);
            message = "Đã cập nhật bình chọn.";
        }

        var diem = await _binhChonRepository.GetDiemBinhChonAsync(loaiDoiTuong, doiTuongId);

        return new BinhChonResponse
        {
            DiemBinhChon = diem,
            BinhChonCuaToi = binhChonCuaToi,
            Message = message
        };
    }

    private async Task EnsureUserActiveAsync(int userId)
    {
        if (!await _nguoiDungRepository.IsActiveAsync(userId))
        {
            throw new InvalidOperationException("Tài khoản của bạn đã bị khóa, không thể thực hiện thao tác này.");
        }
    }
}
