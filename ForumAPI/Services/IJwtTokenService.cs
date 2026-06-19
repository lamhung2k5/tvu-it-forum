using ForumAPI.Models;

namespace ForumAPI.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(NguoiDung user, int expiresInMinutes);
    }
}