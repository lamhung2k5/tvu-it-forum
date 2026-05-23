using System.ComponentModel.DataAnnotations;

namespace ForumAPI.DTOs.CauHoi;

public class CreateCauHoiRequest
{
    [Required(ErrorMessage = "Vui lòng chọn chuyên mục.")]
    public int ID_ChuyenMuc { get; set; }

    [Required(ErrorMessage = "Tiêu đề câu hỏi không được để trống.")]
    [MaxLength(250, ErrorMessage = "Tiêu đề không được vượt quá 250 ký tự.")]
    public string TieuDe { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung câu hỏi không được để trống.")]
    public string NoiDung { get; set; } = string.Empty;
}