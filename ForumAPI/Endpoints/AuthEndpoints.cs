using ForumAPI.DTOs.Auth;
using ForumAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ForumAPI.Endpoints
{
    public static class AuthEndpoints
    {
        // Hàm mở rộng (Extension method) để đăng ký các API vào Program.cs
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            // Tạo một nhóm route chung bắt đầu bằng /api/auth
            var group = app.MapGroup("/api/auth");

            // API Đăng ký: POST /api/auth/register
            group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.RegisterAsync(request);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    // Nếu lỗi (ví dụ trùng email), trả về HTTP 400 kèm thông báo
                    return Results.BadRequest(new { message = ex.Message });
                }
            });

            // API Đăng nhập: POST /api/auth/login
            group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
            {
                try
                {
                    var result = await authService.LoginAsync(request);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    // Nếu sai pass/email hoặc bị khóa, trả về HTTP 400
                    return Results.BadRequest(new { message = ex.Message });
                }
            });
        }
    }
}