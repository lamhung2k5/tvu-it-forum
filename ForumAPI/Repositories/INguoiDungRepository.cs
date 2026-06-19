using ForumAPI.Models;

namespace ForumAPI.Repositories
{
    public interface INguoiDungRepository
    {
        Task<NguoiDung?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<int> CreateAsync(NguoiDung user);
        Task<bool> IsActiveAsync(int userId);   
    }
}