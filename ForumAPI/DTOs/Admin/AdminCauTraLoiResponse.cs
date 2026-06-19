namespace ForumAPI.DTOs.Admin;

public class AdminCauTraLoiResponse
{
    public int ID_CauTraLoi { get; set; }
    public int ID_CauHoi { get; set; }
    public string TieuDeCauHoi { get; set; } = string.Empty;
    public int ID_NguoiDung { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public int DaChapNhan { get; set; }
    public int DiemBinhChon { get; set; }
    public int SoBinhLuan { get; set; }
    public int IsDeleted { get; set; }
    public string NgayTao { get; set; } = string.Empty;
    public string? NgayCapNhat { get; set; }
}
