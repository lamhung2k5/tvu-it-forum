using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumAPI.DTOs.CauHoi;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Endpoints;

public static class CauHoiEndpoints
{
    public static void MapCauHoiEndpoints(this IEndpointRouteBuilder app)
    {
        // Gom nhóm API lại trên Swagger cho đẹp
        var group = app.MapGroup("/api/cauhoi").WithTags("Câu Hỏi");

        // 1. API POST: Đăng câu hỏi mới
        group.MapPost("/", [Authorize] async (
            [FromBody] CreateCauHoiRequest request,
            ICauHoiService cauHoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                // Gọi Service để lưu vào DB
                var newId = await cauHoiService.CreateCauHoiAsync(request, userId);

                return Results.Ok(new
                {
                    Message = "Đăng câu hỏi thành công!",
                    CauHoiId = newId
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

        // 2. API GET: Lấy danh sách câu hỏi, có hỗ trợ tìm kiếm và lọc
        // Ví dụ: /api/cauhoi?keyword=dapper&tag=sqlite&idChuyenMuc=1
        group.MapGet("/", async (
            [FromQuery] string? keyword,
            [FromQuery] string? tag,
            [FromQuery] int? idChuyenMuc,
            ICauHoiService cauHoiService) =>
        {
            var danhSach = await cauHoiService.GetAllCauHoiAsync(keyword, tag, idChuyenMuc);
            return Results.Ok(danhSach);
        });

        // 3. API GET: Lấy chi tiết 1 câu hỏi theo ID
        group.MapGet("/{id}", async (
            int id,
            [FromQuery] bool tangLuotXem,
            ICauHoiService cauHoiService) =>
        {
            var cauHoi = await cauHoiService.GetCauHoiByIdAsync(id, tangLuotXem);

            if (cauHoi == null)
            {
                return Results.NotFound(new { Message = "Không tìm thấy câu hỏi này!" });
            }

            return Results.Ok(cauHoi);
        });

        // 4. API PUT: Chỉnh sửa câu hỏi (Cần đăng nhập)
        group.MapPut("/{id}", [Authorize] async (
            int id,
            [FromBody] UpdateCauHoiRequest request,
            ICauHoiService cauHoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                // Gọi Service sửa bài
                var isSuccess = await cauHoiService.UpdateCauHoiAsync(id, userId, request);

                if (!isSuccess)
                {
                    return Results.BadRequest(new { Message = "Sửa thất bại! Câu hỏi không tồn tại hoặc bạn không có quyền sửa bài của người khác." });
                }

                return Results.Ok(new { Message = "Chỉnh sửa câu hỏi thành công!" });
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

        // 5. API DELETE: Xóa câu hỏi (Xóa mềm - Cần đăng nhập)
        group.MapDelete("/{id}", [Authorize] async (
            int id,
            ICauHoiService cauHoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                // Gọi Service xóa bài
                var isSuccess = await cauHoiService.DeleteCauHoiAsync(id, userId);

                if (!isSuccess)
                {
                    return Results.BadRequest(new { Message = "Xóa thất bại! Câu hỏi không tồn tại, đã bị xóa từ trước hoặc bạn không có quyền xóa." });
                }

                return Results.Ok(new { Message = "Đã xóa câu hỏi thành công!" });
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