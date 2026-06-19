namespace ForumAPI.DTOs.Admin;

public class AdminBinhLuanResponse
{
    public int ID_BinhLuan { get; set; }
    public int ID_NguoiDung { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public int ID_DoiTuong { get; set; }
    public string? TieuDeDoiTuong { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public int IsDeleted { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayCapNhat { get; set; }
}
