using System.ComponentModel.DataAnnotations;

namespace ForumAPI.DTOs.CauTraLoi;

public class CreateCauTraLoiRequest
{
    [Required(ErrorMessage = "Nội dung câu trả lời không được để trống.")]
    public string NoiDung { get; set; } = string.Empty;
}
