namespace ForumAPI.DTOs.Auth
{
    public record RegisterRequest(
        string HoTen, 
        string Email, 
        string Password
    );
}