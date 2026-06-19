using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumAPI.DTOs.BinhLuan;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Endpoints;

public static class BinhLuanEndpoints
{
    public static void MapBinhLuanEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Bình Luận");

        // 1. Bình luận dưới câu hỏi
        group.MapPost("/cauhoi/{id}/binhluan", [Authorize] async (
            int id,
            [FromBody] CreateBinhLuanRequest request,
            IBinhLuanService binhLuanService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var newId = await binhLuanService.CreateBinhLuanCauHoiAsync(id, userId, request);
                return Results.Ok(new
                {
                    Message = "Bình luận câu hỏi thành công!",
                    BinhLuanId = newId
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

        // 2. Lấy danh sách bình luận của câu hỏi
        group.MapGet("/cauhoi/{id}/binhluan", async (
            int id,
            IBinhLuanService binhLuanService) =>
        {
            try
            {
                var danhSach = await binhLuanService.GetBinhLuanCauHoiAsync(id);
                return Results.Ok(danhSach);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { Message = ex.Message });
            }
        });

        // 3. Bình luận dưới câu trả lời
        group.MapPost("/cautraloi/{id}/binhluan", [Authorize] async (
            int id,
            [FromBody] CreateBinhLuanRequest request,
            IBinhLuanService binhLuanService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var newId = await binhLuanService.CreateBinhLuanCauTraLoiAsync(id, userId, request);
                return Results.Ok(new
                {
                    Message = "Bình luận câu trả lời thành công!",
                    BinhLuanId = newId
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

        // 4. Lấy danh sách bình luận của câu trả lời
        group.MapGet("/cautraloi/{id}/binhluan", async (
            int id,
            IBinhLuanService binhLuanService) =>
        {
            try
            {
                var danhSach = await binhLuanService.GetBinhLuanCauTraLoiAsync(id);
                return Results.Ok(danhSach);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { Message = ex.Message });
            }
        });

        // 5. Lấy chi tiết một bình luận
        group.MapGet("/binhluan/{id}", async (
            int id,
            IBinhLuanService binhLuanService) =>
        {
            var binhLuan = await binhLuanService.GetBinhLuanByIdAsync(id);
            if (binhLuan == null)
            {
                return Results.NotFound(new { Message = "Không tìm thấy bình luận này!" });
            }

            return Results.Ok(binhLuan);
        });

        // 6. Sửa bình luận của chính mình
        group.MapPut("/binhluan/{id}", [Authorize] async (
            int id,
            [FromBody] UpdateBinhLuanRequest request,
            IBinhLuanService binhLuanService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var isSuccess = await binhLuanService.UpdateBinhLuanAsync(id, userId, request);
                if (!isSuccess)
                {
                    return Results.BadRequest(new { Message = "Sửa thất bại! Bình luận không tồn tại hoặc bạn không có quyền sửa bình luận của người khác." });
                }

                return Results.Ok(new { Message = "Chỉnh sửa bình luận thành công!" });
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

        // 7. Xóa mềm bình luận của chính mình
        group.MapDelete("/binhluan/{id}", [Authorize] async (
            int id,
            IBinhLuanService binhLuanService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var isSuccess = await binhLuanService.DeleteBinhLuanAsync(id, userId);
                if (!isSuccess)
                {
                    return Results.BadRequest(new { Message = "Xóa thất bại! Bình luận không tồn tại, đã bị xóa từ trước hoặc bạn không có quyền xóa." });
                }

                return Results.Ok(new { Message = "Đã xóa bình luận thành công!" });
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