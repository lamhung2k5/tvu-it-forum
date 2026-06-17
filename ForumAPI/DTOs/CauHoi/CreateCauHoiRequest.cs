using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ForumAPI.DTOs.CauHoi;

public class CreateCauHoiRequest
{
    [Required(ErrorMessage = "Vui lòng chọn chuyên mục.")]
    [JsonPropertyName("idChuyenMuc")]
    public int ID_ChuyenMuc { get; set; }

    [Required(ErrorMessage = "Tiêu đề câu hỏi không được để trống.")]
    [MaxLength(250, ErrorMessage = "Tiêu đề không được vượt quá 250 ký tự.")]
    public string TieuDe { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung câu hỏi không được để trống.")]
    public string NoiDung { get; set; } = string.Empty;

    // Chuỗi thẻ cách nhau bằng dấu phẩy. Ví dụ: "dotnet,dapper,sqlite"
    [JsonPropertyName("the")]
    public string? The { get; set; }
}