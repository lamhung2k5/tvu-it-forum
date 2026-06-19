using ForumAPI.DTOs.User;
using ForumAPI.Repositories;

namespace ForumAPI.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileResponse?> GetMyProfileAsync(int userId)
    {
        return await _userProfileRepository.GetProfileAsync(userId, includeEmail: true);
    }

    public async Task<UserProfileResponse?> GetPublicProfileAsync(int userId)
    {
        return await _userProfileRepository.GetProfileAsync(userId, includeEmail: false);
    }

    public async Task<bool> UpdateMyProfileAsync(int userId, UpdateUserProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.HoTen))
        {
            throw new ArgumentException("Họ tên không được để trống.");
        }

        var hoTen = request.HoTen.Trim();
        if (hoTen.Length < 2 || hoTen.Length > 100)
        {
            throw new ArgumentException("Họ tên phải có độ dài từ 2 đến 100 ký tự.");
        }

        return await _userProfileRepository.UpdateProfileAsync(userId, hoTen);
    }

    public async Task<UserDashboardResponse?> GetMyDashboardAsync(int userId)
    {
        var profile = await _userProfileRepository.GetProfileAsync(userId, includeEmail: true);
        if (profile == null)
        {
            return null;
        }

        var questions = await _userProfileRepository.GetMyQuestionsAsync(
            userId,
            limit: 5,
            includeDeleted: false
        );

        var answers = await _userProfileRepository.GetMyAnswersAsync(
            userId,
            limit: 5,
            includeDeleted: false
        );

        return new UserDashboardResponse
        {
            Profile = profile,
            CauHoiGanDay = questions,
            CauTraLoiGanDay = answers
        };
    }

    public async Task<IEnumerable<UserQuestionResponse>> GetMyQuestionsAsync(int userId)
    {
        return await _userProfileRepository.GetMyQuestionsAsync(
            userId,
            includeDeleted: false
        );
    }

    public async Task<IEnumerable<UserAnswerResponse>> GetMyAnswersAsync(int userId)
    {
        return await _userProfileRepository.GetMyAnswersAsync(
            userId,
            includeDeleted: false
        );
    }

    public async Task<IEnumerable<UserCommentResponse>> GetMyCommentsAsync(int userId)
    {
        return await _userProfileRepository.GetMyCommentsAsync(userId);
    }

    public async Task<IEnumerable<UserQuestionResponse>> GetPublicQuestionsAsync(int userId)
    {
        return await _userProfileRepository.GetPublicQuestionsAsync(userId);
    }

    public async Task<IEnumerable<UserAnswerResponse>> GetPublicAnswersAsync(int userId)
    {
        return await _userProfileRepository.GetPublicAnswersAsync(userId);
    }
}
