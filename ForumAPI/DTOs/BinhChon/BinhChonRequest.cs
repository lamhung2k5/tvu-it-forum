using System.ComponentModel.DataAnnotations;

namespace ForumAPI.DTOs.BinhChon;

public class BinhChonRequest
{
    [Required(ErrorMessage = "Giá trị bình chọn không được để trống.")]
    public int GiaTri { get; set; }
}
