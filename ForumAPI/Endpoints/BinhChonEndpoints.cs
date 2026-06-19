using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumAPI.DTOs.BinhChon;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Endpoints;

public static class BinhChonEndpoints
{
    public static void MapBinhChonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Bình Chọn");

        // 1. Bình chọn câu hỏi
        group.MapPost("/cauhoi/{id}/binhchon", [Authorize] async (
            int id,
            [FromBody] BinhChonRequest request,
            IBinhChonService binhChonService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var result = await binhChonService.BinhChonCauHoiAsync(id, userId, request);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        });

        // 2. Bình chọn câu trả lời
        group.MapPost("/cautraloi/{id}/binhchon", [Authorize] async (
            int id,
            [FromBody] BinhChonRequest request,
            IBinhChonService binhChonService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var result = await binhChonService.BinhChonCauTraLoiAsync(id, userId, request);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
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