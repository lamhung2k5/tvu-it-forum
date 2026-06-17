namespace ForumAPI.DTOs.User;

public class UserProfileResponse
{
    public int ID_NguoiDung { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? AnhDaiDien { get; set; }
    public string VaiTro { get; set; } = "User";
    public int TrangThai { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    public int SoCauHoi { get; set; }
    public int SoCauTraLoi { get; set; }
    public int SoBinhLuan { get; set; }
    public int SoCauTraLoiDuocChapNhan { get; set; }
    public int TongDiemBinhChon { get; set; }
}
