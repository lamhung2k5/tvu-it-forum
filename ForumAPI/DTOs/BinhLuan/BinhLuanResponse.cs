namespace ForumAPI.DTOs.BinhLuan;

public class BinhLuanResponse
{
    public int ID_BinhLuan { get; set; }
    public int ID_NguoiDung { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public int ID_DoiTuong { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayCapNhat { get; set; }
}
