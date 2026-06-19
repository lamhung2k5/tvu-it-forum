using ForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ForumAPI.DTOs.Admin;

namespace ForumAPI.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin")
            .WithTags("Admin")
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

        // 0. Tổng quan quản trị: số liệu và dữ liệu biểu đồ
        group.MapGet("/dashboard", async (IAdminService adminService) =>
        {
            var dashboard = await adminService.GetDashboardAsync();
            return Results.Ok(dashboard);
        });

        // 1. Xem danh sách người dùng
        group.MapGet("/users", async (
            [FromQuery] string? keyword,
            IAdminService adminService) =>
        {
            var users = await adminService.GetUsersAsync(keyword);
            return Results.Ok(users);
        });

        // 2. Xem danh sách câu hỏi, mặc định gồm cả bài đã xóa mềm
        // Ví dụ: /api/admin/cauhoi?keyword=dapper&isDeleted=1
        group.MapGet("/cauhoi", async (
            [FromQuery] string? keyword,
            [FromQuery] int? isDeleted,
            IAdminService adminService) =>
        {
            var cauHoi = await adminService.GetCauHoiAsync(keyword, isDeleted);
            return Results.Ok(cauHoi);
        });

        // 3. Admin xóa mềm câu hỏi vi phạm
        group.MapDelete("/cauhoi/{id}", async (
            int id,
            IAdminService adminService) =>
        {
            var isSuccess = await adminService.DeleteCauHoiAsync(id);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Xóa thất bại! Câu hỏi không tồn tại hoặc đã bị xóa từ trước." });
            }

            return Results.Ok(new { Message = "Admin đã xóa mềm câu hỏi thành công." });
        });

        // 4. Admin khôi phục câu hỏi
        group.MapPatch("/cauhoi/{id}/khoiphuc", async (
            int id,
            IAdminService adminService) =>
        {
            var isSuccess = await adminService.RestoreCauHoiAsync(id);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Khôi phục thất bại! Câu hỏi không tồn tại hoặc chưa bị xóa." });
            }

            return Results.Ok(new { Message = "Admin đã khôi phục câu hỏi thành công." });
        });

        // 5. Xem danh sách câu trả lời, mặc định gồm cả câu trả lời đã xóa mềm
        // Ví dụ: /api/admin/cautraloi?cauHoiId=1&isDeleted=0
        group.MapGet("/cautraloi", async (
            [FromQuery] int? cauHoiId,
            [FromQuery] int? isDeleted,
            IAdminService adminService) =>
        {
            var cauTraLoi = await adminService.GetCauTraLoiAsync(cauHoiId, isDeleted);
            return Results.Ok(cauTraLoi);
        });

        // 6. Admin xóa mềm câu trả lời vi phạm
        group.MapDelete("/cautraloi/{id}", async (
            int id,
            IAdminService adminService) =>
        {
            var isSuccess = await adminService.DeleteCauTraLoiAsync(id);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Xóa thất bại! Câu trả lời không tồn tại hoặc đã bị xóa từ trước." });
            }

            return Results.Ok(new { Message = "Admin đã xóa mềm câu trả lời thành công." });
        });

        // 7. Admin khôi phục câu trả lời
        group.MapPatch("/cautraloi/{id}/khoiphuc", async (
            int id,
            IAdminService adminService) =>
        {
            var isSuccess = await adminService.RestoreCauTraLoiAsync(id);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Khôi phục thất bại! Câu trả lời không tồn tại hoặc chưa bị xóa." });
            }

            return Results.Ok(new { Message = "Admin đã khôi phục câu trả lời thành công." });
        });

        // 8. Xem danh sách bình luận, mặc định gồm cả bình luận đã xóa mềm
        // Ví dụ: /api/admin/binhluan?loaiDoiTuong=CAUHOI&isDeleted=1
        group.MapGet("/binhluan", async (
            [FromQuery] string? loaiDoiTuong,
            [FromQuery] int? isDeleted,
            IAdminService adminService) =>
        {
            try
            {
                var binhLuan = await adminService.GetBinhLuanAsync(loaiDoiTuong, isDeleted);
                return Results.Ok(binhLuan);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        });

        // 9. Admin xóa mềm bình luận vi phạm
        group.MapDelete("/binhluan/{id}", async (
            int id,
            IAdminService adminService) =>
        {
            var isSuccess = await adminService.DeleteBinhLuanAsync(id);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Xóa thất bại! Bình luận không tồn tại hoặc đã bị xóa từ trước." });
            }

            return Results.Ok(new { Message = "Admin đã xóa mềm bình luận thành công." });
        });

        // 10. Admin khôi phục bình luận
        group.MapPatch("/binhluan/{id}/khoiphuc", async (
            int id,
            IAdminService adminService) =>
        {
            var isSuccess = await adminService.RestoreBinhLuanAsync(id);
            if (!isSuccess)
            {
                return Results.BadRequest(new { Message = "Khôi phục thất bại! Bình luận không tồn tại hoặc chưa bị xóa." });
            }

            return Results.Ok(new { Message = "Admin đã khôi phục bình luận thành công." });
        });

        group.MapPatch("/users/{id:int}/lock", async (
            int id,
            ClaimsPrincipal user,
            IAdminService adminService) =>
        {
            try
            {
                var currentUserId = GetCurrentUserId(user);
                await adminService.KhoaNguoiDungAsync(id, currentUserId);

                return Results.Ok(new { message = "Đã khóa tài khoản người dùng." });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        });

        group.MapPatch("/users/{id:int}/unlock", async (
            int id,
            IAdminService adminService) =>
        {
            try
            {
                await adminService.MoKhoaNguoiDungAsync(id);

                return Results.Ok(new { message = "Đã mở khóa tài khoản người dùng." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        });

        group.MapPatch("/users/{id:int}/role", async (
            int id,
            CapNhatVaiTroRequest request,
            ClaimsPrincipal user,
            IAdminService adminService) =>
        {
            try
            {
                var currentUserId = GetCurrentUserId(user);
                await adminService.CapNhatVaiTroNguoiDungAsync(id, request.VaiTro, currentUserId);

                return Results.Ok(new { message = "Đã cập nhật vai trò người dùng." });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
        });

        static int? GetCurrentUserId(ClaimsPrincipal user)
        {
            var rawId =
                user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("ID_NguoiDung")
                ?? user.FindFirstValue("idNguoiDung")
                ?? user.FindFirstValue("sub");

            return int.TryParse(rawId, out var id) ? id : null;
        }
    }
}
