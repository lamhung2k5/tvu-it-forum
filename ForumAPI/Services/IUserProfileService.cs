using ForumAPI.DTOs.User;

namespace ForumAPI.Services;

public interface IUserProfileService
{
    Task<UserProfileResponse?> GetMyProfileAsync(int userId);
    Task<UserProfileResponse?> GetPublicProfileAsync(int userId);
    Task<bool> UpdateMyProfileAsync(int userId, UpdateUserProfileRequest request);
    Task<UserDashboardResponse?> GetMyDashboardAsync(int userId);
    Task<IEnumerable<UserQuestionResponse>> GetMyQuestionsAsync(int userId);
    Task<IEnumerable<UserAnswerResponse>> GetMyAnswersAsync(int userId);
    Task<IEnumerable<UserCommentResponse>> GetMyCommentsAsync(int userId);

    Task<IEnumerable<UserQuestionResponse>> GetPublicQuestionsAsync(int userId);
    Task<IEnumerable<UserAnswerResponse>> GetPublicAnswersAsync(int userId);
}
