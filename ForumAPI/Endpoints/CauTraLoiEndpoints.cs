using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ForumAPI.DTOs.CauTraLoi;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumAPI.Endpoints;

public static class CauTraLoiEndpoints
{
    public static void MapCauTraLoiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Câu Trả Lời");

        // 1. API POST: Thêm câu trả lời cho một câu hỏi
        group.MapPost("/cauhoi/{id}/cautraloi", [Authorize] async (
            int id,
            [FromBody] CreateCauTraLoiRequest request,
            ICauTraLoiService cauTraLoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var newId = await cauTraLoiService.CreateCauTraLoiAsync(id, userId, request);
                return Results.Ok(new
                {
                    Message = "Trả lời câu hỏi thành công!",
                    CauTraLoiId = newId
                });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { Message = ex.Message });
            }
        });

        // 2. API GET: Lấy danh sách câu trả lời của một câu hỏi
        group.MapGet("/cauhoi/{id}/cautraloi", async (
            int id,
            ICauTraLoiService cauTraLoiService) =>
        {
            try
            {
                var danhSach = await cauTraLoiService.GetCauTraLoiByCauHoiIdAsync(id);
                return Results.Ok(danhSach);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { Message = ex.Message });
            }
        });

        // 3. API GET: Lấy chi tiết một câu trả lời theo ID
        group.MapGet("/cautraloi/{id}", async (
            int id,
            ICauTraLoiService cauTraLoiService) =>
        {
            var cauTraLoi = await cauTraLoiService.GetCauTraLoiByIdAsync(id);
            if (cauTraLoi == null)
            {
                return Results.NotFound(new { Message = "Không tìm thấy câu trả lời này!" });
            }

            return Results.Ok(cauTraLoi);
        });

        // 4. API PUT: Chỉnh sửa câu trả lời của chính mình
        group.MapPut("/cautraloi/{id}", [Authorize] async (
            int id,
            [FromBody] UpdateCauTraLoiRequest request,
            ICauTraLoiService cauTraLoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            try
            {
                var isSuccess = await cauTraLoiService.UpdateCauTraLoiAsync(id, userId, request);
                if (!isSuccess)
                {
                    return Results.BadRequest(new { Message = "Sửa thất bại! Câu trả lời không tồn tại hoặc bạn không có quyền sửa câu trả lời của người khác." });
                }

                return Results.Ok(new { Message = "Chỉnh sửa câu trả lời thành công!" });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        });

        // 5. API DELETE: Xóa mềm câu trả lời của chính mình
        group.MapDelete("/cautraloi/{id}", [Authorize] async (
            int id,
            ICauTraLoiService cauTraLoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var isSuccess = await cauTraLoiService.DeleteCauTraLoiAsync(id, userId);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Xóa thất bại! Câu trả lời không tồn tại, đã bị xóa từ trước hoặc bạn không có quyền xóa." });
            }

            return Results.Ok(new { Message = "Đã xóa câu trả lời thành công!" });
        });

        // 6. API PATCH: Chủ câu hỏi chọn câu trả lời được chấp nhận
        group.MapPatch("/cautraloi/{id}/chapnhan", [Authorize] async (
            int id,
            ICauTraLoiService cauTraLoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var isSuccess = await cauTraLoiService.AcceptCauTraLoiAsync(id, userId);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Chọn câu trả lời thất bại! Câu trả lời không tồn tại hoặc bạn không phải chủ câu hỏi." });
            }

            return Results.Ok(new { Message = "Đã chọn câu trả lời được chấp nhận!" });
        });

        // 7. API PATCH: Chủ câu hỏi bỏ chấp nhận câu trả lời
        group.MapPatch("/cautraloi/{id}/bo-chap-nhan", [Authorize] async (
            int id,
            ICauTraLoiService cauTraLoiService,
            ClaimsPrincipal user) =>
        {
            if (!TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var isSuccess = await cauTraLoiService.UnacceptAnswerAsync(id, userId);

            if (!isSuccess)
            {
                return Results.BadRequest(new
                {
                    Message = "Thu hồi thất bại! Câu trả lời không tồn tại, chưa được chấp nhận hoặc bạn không phải chủ câu hỏi."
                });
            }

            return Results.Ok(new
            {
                Message = "Đã thu hồi câu trả lời được chấp nhận."
            });
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
