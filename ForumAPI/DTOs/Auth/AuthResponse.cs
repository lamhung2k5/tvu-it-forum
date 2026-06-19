namespace ForumAPI.DTOs.Auth
{
    // Thông tin user an toàn để trả về
    public record UserResponse(
        int ID_NguoiDung, 
        string HoTen, 
        string Email, 
        string? AnhDaiDien, 
        string VaiTro, 
        int TrangThai
    );

    // Response tổng bao gồm cả Token
    public record AuthResponse(
        string AccessToken, 
        int ExpiresIn, 
        UserResponse User
    );
}