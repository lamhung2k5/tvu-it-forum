namespace ForumAPI.Models
{
    public class NguoiDung
    {
        public int ID_NguoiDung { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MatKhauHash { get; set; } = string.Empty;
        public string? AnhDaiDien { get; set; }
        public string VaiTro { get; set; } = "User";
        
        // 1: Hoạt động, 0: Bị khóa
        public int TrangThai { get; set; } = 1; 
        
        // Lưu dưới dạng chuỗi chuẩn ISO 8601 (yyyy-MM-dd HH:mm:ss)
        public string NgayTao { get; set; } = string.Empty;
        public string? NgayCapNhat { get; set; }
    }
}