using System.Text;
using ForumAPI.Data;
using ForumAPI.Endpoints;
using ForumAPI.Repositories;
using ForumAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ DEPENDENCY INJECTION (DI) ---
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddTransient<DatabaseInitializer>();
builder.Services.AddTransient<DatabaseSeeder>();    
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Đăng ký cho khối Câu hỏi (Sprint 2)
builder.Services.AddScoped<ICauHoiRepository, CauHoiRepository>();
builder.Services.AddScoped<ICauHoiService, CauHoiService>();

// Đăng ký cho khối Câu trả lời (Sprint 3)
builder.Services.AddScoped<ICauTraLoiRepository, CauTraLoiRepository>();
builder.Services.AddScoped<ICauTraLoiService, CauTraLoiService>();

// Đăng ký cho khối Bình chọn (Sprint 4)
builder.Services.AddScoped<IBinhChonRepository, BinhChonRepository>();
builder.Services.AddScoped<IBinhChonService, BinhChonService>();

// Đăng ký cho khối Bình luận (Sprint 5)
builder.Services.AddScoped<IBinhLuanRepository, BinhLuanRepository>();
builder.Services.AddScoped<IBinhLuanService, BinhLuanService>();

// Đăng ký cho khối Admin (Sprint 6)
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Đăng ký cho khối Profile/Dashboard người dùng (Sprint 7 mở rộng)
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// Đăng ký cho khối Thông báo và Tố cáo
builder.Services.AddScoped<IThongBaoRepository, ThongBaoRepository>();
builder.Services.AddScoped<IThongBaoService, ThongBaoService>();
builder.Services.AddScoped<IToCaoRepository, ToCaoRepository>();
builder.Services.AddScoped<IToCaoService, ToCaoService>();

// Cấu hình CORS để frontend Vue có thể gọi API khi chạy khác port
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowForumWeb", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Cấu hình Swagger/OpenAPI (Giao diện cực tiện để test API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Tạo nút Authorize (ổ khóa) trên Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo cú pháp: Bearer [Khoảng trắng] [Chuỗi Token của bạn]",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Ép Swagger phải đính kèm Token này vào mỗi lần gửi API đi
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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

    var dbSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    dbSeeder.Seed();
}

// --- 4. CẤU HÌNH MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowForumWeb");

app.UseAuthentication(); // Bắt buộc phải gọi trước UseAuthorization
app.UseAuthorization();

// --- 5. MAP ENDPOINT ---
app.MapAuthEndpoints();

app.MapCauHoiEndpoints();

app.MapCauTraLoiEndpoints();

app.MapBinhChonEndpoints();

app.MapBinhLuanEndpoints();

app.MapAdminEndpoints();

app.MapUserEndpoints();

app.MapThongBaoEndpoints();

app.MapToCaoEndpoints();

app.Run();