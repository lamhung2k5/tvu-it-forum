namespace ForumAPI.DTOs.CauHoi;

public class CauHoiResponse
{
    public int ID_CauHoi { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public int LuotXem { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    
    // Thuộc tính này lấy từ bảng CHUYENMUC thông qua JOIN
    public string TenChuyenMuc { get; set; } = string.Empty; 
}