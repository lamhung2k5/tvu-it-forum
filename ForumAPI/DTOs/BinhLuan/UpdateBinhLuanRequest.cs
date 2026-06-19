using System.ComponentModel.DataAnnotations;

namespace ForumAPI.DTOs.BinhLuan;

public class UpdateBinhLuanRequest
{
    [Required(ErrorMessage = "Nội dung bình luận không được để trống.")]
    public string NoiDung { get; set; } = string.Empty;
}
