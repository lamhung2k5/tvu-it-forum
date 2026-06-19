namespace ForumAPI.Models;

public class ToCao
{
    public int ID_ToCao { get; set; }
    public int ID_NguoiToCao { get; set; }
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public int ID_DoiTuong { get; set; }
    public string LyDo { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public string TrangThai { get; set; } = "PENDING";
    public int? ID_AdminXuLy { get; set; }
    public string? GhiChuXuLy { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayXuLy { get; set; }
}
