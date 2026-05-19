using ForumAPI.DTOs.Auth;

namespace ForumAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> RegisterAsync(RegisterRequest request);
    }
}