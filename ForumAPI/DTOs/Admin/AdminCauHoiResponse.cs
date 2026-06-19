namespace ForumAPI.DTOs.Admin;

public class AdminCauHoiResponse
{
    public int ID_CauHoi { get; set; }
    public int ID_NguoiDung { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public int ID_ChuyenMuc { get; set; }
    public string TenChuyenMuc { get; set; } = string.Empty;
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public int TrangThai { get; set; }
    public int LuotXem { get; set; }
    public int DiemBinhChon { get; set; }
    public int SoCauTraLoi { get; set; }
    public int SoBinhLuan { get; set; }
    public string? Tags { get; set; }
    public int IsDeleted { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayCapNhat { get; set; }
}
