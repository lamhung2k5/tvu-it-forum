using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumAPI.DTOs.ToCao;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Endpoints;

public static class ToCaoEndpoints
{
    public static void MapToCaoEndpoints(this IEndpointRouteBuilder app)
    {
        var userGroup = app.MapGroup("/api/tocao")
            .WithTags("Tố cáo")
            .RequireAuthorization();

        userGroup.MapPost("/", async (
            [FromBody] CreateToCaoRequest request,
            IToCaoService toCaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var newId = await toCaoService.CreateAsync(userId, request);
                return Results.Ok(new
                {
                    Message = "Đã gửi tố cáo đến quản trị viên.",
                    ID_ToCao = newId
                });
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

        var adminGroup = app.MapGroup("/api/admin/tocao")
            .WithTags("Admin - Tố cáo")
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

        adminGroup.MapGet("/", async (
            [FromQuery] string? trangThai,
            IToCaoService toCaoService) =>
        {
            try
            {
                var reports = await toCaoService.GetAllAsync(trangThai);
                return Results.Ok(reports);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        });

        adminGroup.MapGet("/pending-count", async (IToCaoService toCaoService) =>
        {
            var count = await toCaoService.CountPendingAsync();
            return Results.Ok(new { Count = count });
        });

        adminGroup.MapPatch("/{id:int}/reject", async (
            int id,
            [FromBody] XuLyToCaoRequest request,
            IToCaoService toCaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var adminId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var success = await toCaoService.RejectAsync(id, adminId, request);
                return success
                    ? Results.Ok(new { Message = "Đã bỏ qua tố cáo." })
                    : Results.NotFound(new { Message = "Không tìm thấy tố cáo." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { Message = ex.Message });
            }
        });

        adminGroup.MapPatch("/{id:int}/remind", async (
            int id,
            [FromBody] XuLyToCaoRequest request,
            IToCaoService toCaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var adminId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var success = await toCaoService.RemindAsync(id, adminId, request);
                return success
                    ? Results.Ok(new { Message = "Đã nhắc nhở người dùng liên quan." })
                    : Results.NotFound(new { Message = "Không tìm thấy tố cáo." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { Message = ex.Message });
            }
        });

        adminGroup.MapPatch("/{id:int}/resolve", async (
            int id,
            [FromBody] XuLyToCaoRequest request,
            IToCaoService toCaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var adminId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var success = await toCaoService.ResolveAsync(id, adminId, request);
                return success
                    ? Results.Ok(new { Message = "Đã xử lý tố cáo và xóa mềm nội dung." })
                    : Results.NotFound(new { Message = "Không tìm thấy tố cáo." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { Message = ex.Message });
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
