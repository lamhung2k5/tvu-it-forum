namespace ForumAPI.Models;

public class BinhLuan
{
    public int ID_BinhLuan { get; set; }
    public int ID_NguoiDung { get; set; }
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public int ID_DoiTuong { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public int IsDeleted { get; set; } = 0;
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayCapNhat { get; set; }
}
