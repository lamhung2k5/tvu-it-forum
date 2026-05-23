namespace ForumAPI.Models;

public class CauHoi
{
    public int ID_CauHoi { get; set; }
    
    public int ID_NguoiDung { get; set; }
    
    public int ID_ChuyenMuc { get; set; }
    
    public string TieuDe { get; set; } = string.Empty;
    
    public string NoiDung { get; set; } = string.Empty;
    
    public int TrangThai { get; set; } = 0;
    
    public int LuotXem { get; set; } = 0;
    
    // SQLite thường lưu thời gian dưới dạng chuỗi TEXT
    public string NgayTao { get; set; } = string.Empty; 
    
    public string? NgayCapNhat { get; set; }
    
    public int IsDeleted { get; set; } = 0;
}