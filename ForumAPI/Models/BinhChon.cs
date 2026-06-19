namespace ForumAPI.Models;

public class BinhChon
{
    public int ID_BinhChon { get; set; }

    public int ID_NguoiDung { get; set; }

    // CAUHOI hoặc CAUTRALOI
    public string LoaiDoiTuong { get; set; } = string.Empty;

    public int ID_DoiTuong { get; set; }

    // 1: upvote, -1: downvote
    public int GiaTri { get; set; }

    public string NgayTao { get; set; } = string.Empty;

    public string? NgayCapNhat { get; set; }
}
