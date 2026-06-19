namespace ForumAPI.Models;

public class ThongBao
{
    public int ID_ThongBao { get; set; }
    public int ID_NguoiNhan { get; set; }
    public int? ID_NguoiTao { get; set; }
    public string LoaiThongBao { get; set; } = string.Empty;
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string? Link { get; set; }
    public int DaDoc { get; set; }
    public int DaXoa { get; set; }
    public string NgayTao { get; set; } = string.Empty;
}
