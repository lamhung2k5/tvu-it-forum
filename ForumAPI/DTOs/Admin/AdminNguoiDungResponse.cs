namespace ForumAPI.DTOs.Admin;

public class AdminNguoiDungResponse
{
    public int ID_NguoiDung { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public string VaiTro { get; set; } = string.Empty;
    public int TrangThai { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayCapNhat { get; set; }
}
