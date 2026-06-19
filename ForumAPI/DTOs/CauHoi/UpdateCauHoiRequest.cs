using System.Text.Json.Serialization;

namespace ForumAPI.DTOs.CauHoi;

public class UpdateCauHoiRequest
{
    [JsonPropertyName("idChuyenMuc")]
    public int ID_ChuyenMuc { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;

    // Gửi null để giữ nguyên thẻ, gửi chuỗi rỗng để xóa hết thẻ.
    [JsonPropertyName("the")]
    public string? The { get; set; }
}
