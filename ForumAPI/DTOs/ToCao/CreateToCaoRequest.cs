namespace ForumAPI.DTOs.ToCao;

public class CreateToCaoRequest
{
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public int ID_DoiTuong { get; set; }
    public string LyDo { get; set; } = string.Empty;
    public string? MoTa { get; set; }
}
