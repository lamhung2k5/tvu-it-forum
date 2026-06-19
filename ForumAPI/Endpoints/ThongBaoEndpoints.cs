using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Endpoints;

public static class ThongBaoEndpoints
{
    public static void MapThongBaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/thongbao")
            .WithTags("Thông báo")
            .RequireAuthorization();

        group.MapGet("/recent", async (
            [FromQuery] int? limit,
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var danhSach = await thongBaoService.GetRecentUnreadAsync(userId, limit ?? 5);
            return Results.Ok(danhSach);
        });

        group.MapGet("/", async (
            [FromQuery] string? status,
            [FromQuery] string? category,
            [FromQuery] string? timeRange,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var currentPage = Math.Max(page ?? 1, 1);
            var currentPageSize = Math.Clamp(pageSize ?? 10, 1, 50);

            var items = await thongBaoService.GetMyNotificationsAsync(userId, status, category, timeRange, currentPage, currentPageSize);
            var total = await thongBaoService.CountMyNotificationsAsync(userId, status, category, timeRange);

            return Results.Ok(new
            {
                Items = items,
                Total = total,
                Page = currentPage,
                PageSize = currentPageSize
            });
        });

        group.MapGet("/unread-count", async (
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var count = await thongBaoService.CountUnreadAsync(userId);
            return Results.Ok(new { Count = count });
        });

        group.MapPatch("/{id:int}/read", async (
            int id,
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var success = await thongBaoService.MarkAsReadAsync(id, userId);
            if (!success)
            {
                return Results.NotFound(new { Message = "Không tìm thấy thông báo." });
            }

            return Results.Ok(new { Message = "Đã đánh dấu thông báo là đã đọc." });
        });

        group.MapPatch("/read-all", async (
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var count = await thongBaoService.MarkAllAsReadAsync(userId);
            return Results.Ok(new { Message = "Đã đánh dấu tất cả thông báo là đã đọc.", Count = count });
        });

        group.MapDelete("/{id:int}", async (
            int id,
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var success = await thongBaoService.SoftDeleteAsync(id, userId);
            if (!success)
            {
                return Results.NotFound(new { Message = "Không tìm thấy thông báo cần xóa." });
            }

            return Results.Ok(new { Message = "Đã xóa thông báo." });
        });

        group.MapDelete("/clear-read", async (
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var count = await thongBaoService.ClearReadAsync(userId);
            return Results.Ok(new { Message = "Đã dọn thông báo đã đọc.", Count = count });
        });

        group.MapDelete("/clear-all", async (
            IThongBaoService thongBaoService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var count = await thongBaoService.ClearAllAsync(userId);
            return Results.Ok(new { Message = "Đã xóa tất cả thông báo.", Count = count });
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
