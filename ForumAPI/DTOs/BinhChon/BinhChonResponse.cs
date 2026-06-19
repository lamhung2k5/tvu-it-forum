namespace ForumAPI.DTOs.BinhChon;

public class BinhChonResponse
{
    public int DiemBinhChon { get; set; }

    // 1: người dùng đang upvote, -1: đang downvote, 0: chưa vote hoặc đã hủy vote
    public int BinhChonCuaToi { get; set; }

    public string Message { get; set; } = string.Empty;
}
