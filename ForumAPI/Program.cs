using System.Text;
using ForumAPI.Data;
using ForumAPI.Endpoints;
using ForumAPI.Repositories;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ DEPENDENCY INJECTION (DI) ---
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddTransient<DatabaseInitializer>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Cấu hình Swagger/OpenAPI (Giao diện cực tiện để test API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. CẤU HÌNH JWT AUTHENTICATION ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Thiếu cấu hình Jwt:Key trong appsettings.json");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- 3. KHỞI TẠO CƠ SỞ DỮ LIỆU ---
// Chạy lệnh này mỗi khi app khởi động để đảm bảo file forum.db và bảng NGUOIDUNG luôn sẵn sàng
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    dbInitializer.Initialize();
}

// --- 4. CẤU HÌNH MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Bắt buộc phải gọi trước UseAuthorization
app.UseAuthorization();

// --- 5. MAP ENDPOINT ---
app.MapAuthEndpoints();

app.Run();