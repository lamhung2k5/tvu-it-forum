using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumAPI.DTOs.User;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Người Dùng / Profile");

        group.MapGet("/me", [Authorize] async (
            IUserProfileService userProfileService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var profile = await userProfileService.GetMyProfileAsync(userId);
            return profile == null
                ? Results.NotFound(new { Message = "Không tìm thấy thông tin người dùng." })
                : Results.Ok(profile);
        });

        group.MapPut("/me", [Authorize] async (
            [FromBody] UpdateUserProfileRequest request,
            IUserProfileService userProfileService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var isSuccess = await userProfileService.UpdateMyProfileAsync(userId, request);
                return isSuccess
                    ? Results.Ok(new { Message = "Cập nhật hồ sơ thành công." })
                    : Results.BadRequest(new { Message = "Cập nhật hồ sơ thất bại." });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        });

        group.MapGet("/me/dashboard", [Authorize] async (
            IUserProfileService userProfileService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var dashboard = await userProfileService.GetMyDashboardAsync(userId);
            return dashboard == null
                ? Results.NotFound(new { Message = "Không tìm thấy dashboard người dùng." })
                : Results.Ok(dashboard);
        });

        group.MapGet("/me/questions", [Authorize] async (
            IUserProfileService userProfileService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var questions = await userProfileService.GetMyQuestionsAsync(userId);
            return Results.Ok(questions);
        });

        group.MapGet("/me/answers", [Authorize] async (
            IUserProfileService userProfileService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var answers = await userProfileService.GetMyAnswersAsync(userId);
            return Results.Ok(answers);
        });

        group.MapGet("/me/comments", [Authorize] async (
            IUserProfileService userProfileService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var comments = await userProfileService.GetMyCommentsAsync(userId);
            return Results.Ok(comments);
        });

        group.MapGet("/{id}/profile", async (int id, IUserProfileService userProfileService) =>
        {
            var profile = await userProfileService.GetPublicProfileAsync(id);
            return profile == null
                ? Results.NotFound(new { Message = "Không tìm thấy hồ sơ người dùng." })
                : Results.Ok(profile);
        });
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out int userId)
    {
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("id")?.Value;

        return int.TryParse(userIdString, out userId);
    }
}
