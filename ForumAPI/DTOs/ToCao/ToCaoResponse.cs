namespace ForumAPI.DTOs.ToCao;

public class ToCaoResponse
{
    public int ID_ToCao { get; set; }
    public int ID_NguoiToCao { get; set; }
    public string NguoiToCao { get; set; } = string.Empty;
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public int ID_DoiTuong { get; set; }
    public string LyDo { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public int? ID_AdminXuLy { get; set; }
    public string? AdminXuLy { get; set; }
    public string? GhiChuXuLy { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayXuLy { get; set; }
    public int? ID_NguoiBiToCao { get; set; }
    public string? NguoiBiToCao { get; set; }
    public string? NoiDungBiToCao { get; set; }
    public string? Link { get; set; }
    public int IsDeleted { get; set; }
}
