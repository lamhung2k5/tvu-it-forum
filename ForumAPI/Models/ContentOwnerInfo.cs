namespace ForumAPI.Models;

public class ContentOwnerInfo
{
    public int ID_DoiTuong { get; set; }
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public int ID_NguoiDung { get; set; }
    public int ID_CauHoi { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public int IsDeleted { get; set; }
}
