namespace ForumAPI.Models;

public class CauTraLoi
{
    public int ID_CauTraLoi { get; set; }

    public int ID_CauHoi { get; set; }

    public int ID_NguoiDung { get; set; }

    public string NoiDung { get; set; } = string.Empty;

    // 1: Câu trả lời được người đăng câu hỏi chấp nhận, 0: bình thường
    public int DaChapNhan { get; set; } = 0;

    // 0: còn hiển thị, 1: đã xóa mềm
    public int IsDeleted { get; set; } = 0;

    public string NgayTao { get; set; } = string.Empty;

    public string? NgayCapNhat { get; set; }
}
