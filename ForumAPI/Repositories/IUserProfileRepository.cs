using ForumAPI.DTOs.User;

namespace ForumAPI.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfileResponse?> GetProfileAsync(int userId, bool includeEmail);
    Task<bool> UpdateProfileAsync(int userId, string hoTen);
    Task<IEnumerable<UserQuestionResponse>> GetMyQuestionsAsync(int userId, int limit = 0, bool includeDeleted = false);
    Task<IEnumerable<UserAnswerResponse>> GetMyAnswersAsync(int userId, int limit = 0, bool includeDeleted = false);
    Task<IEnumerable<UserCommentResponse>> GetMyCommentsAsync(int userId, int limit = 0);

    Task<IEnumerable<UserQuestionResponse>> GetPublicQuestionsAsync(int userId);
    Task<IEnumerable<UserAnswerResponse>> GetPublicAnswersAsync(int userId);
}
