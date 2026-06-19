namespace ForumAPI.Models;

public class ChuyenMuc
{
    public int ID_ChuyenMuc { get; set; }
    public string TenChuyenMuc { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public int TrangThai { get; set; } = 1;
}