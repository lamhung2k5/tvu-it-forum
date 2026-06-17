using ForumAPI.DTOs.CauHoi;

namespace ForumAPI.DTOs.User;

public class UserDashboardResponse
{
    public UserProfileResponse Profile { get; set; } = new();
    public IEnumerable<UserQuestionResponse> CauHoiGanDay { get; set; } = Enumerable.Empty<UserQuestionResponse>();
    public IEnumerable<UserAnswerResponse> CauTraLoiGanDay { get; set; } = Enumerable.Empty<UserAnswerResponse>();
}
