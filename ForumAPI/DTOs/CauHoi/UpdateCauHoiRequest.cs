namespace ForumAPI.DTOs.CauHoi;

public class UpdateCauHoiRequest
{
    public int ID_ChuyenMuc { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
}