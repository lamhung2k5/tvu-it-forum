using ForumAPI.DTOs.BinhChon;

namespace ForumAPI.Services;

public interface IBinhChonService
{
    Task<BinhChonResponse> BinhChonCauHoiAsync(int cauHoiId, int userId, BinhChonRequest request);
    Task<BinhChonResponse> BinhChonCauTraLoiAsync(int cauTraLoiId, int userId, BinhChonRequest request);
}
