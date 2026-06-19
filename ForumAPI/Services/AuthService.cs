using ForumAPI.DTOs.Auth;
using ForumAPI.Models;
using ForumAPI.Repositories;
using Microsoft.Extensions.Configuration;

namespace ForumAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly INguoiDungRepository _userRepo;
        private readonly IJwtTokenService _jwtService;
        private readonly IConfiguration _config;

        public AuthService(INguoiDungRepository userRepo, IJwtTokenService jwtService, IConfiguration config)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _config = config;
        }

        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepo.EmailExistsAsync(request.Email))
            {
                // Tạm ném Exception, thực tế có thể dùng Result Pattern
                throw new Exception("Email đã tồn tại trong hệ thống.");
            }

            var user = new NguoiDung
            {
                HoTen = request.HoTen,
                Email = request.Email,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var newId = await _userRepo.CreateAsync(user);
            
            return new UserResponse(
                ID_NguoiDung: newId,
                HoTen: user.HoTen,
                Email: user.Email,
                AnhDaiDien: null,
                VaiTro: user.VaiTro,
                TrangThai: user.TrangThai
            );
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email) 
                ?? throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.MatKhauHash))
            {
                throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");
            }

            if (user.TrangThai == 0)
            {
                throw new Exception("Tài khoản này đã bị khóa.");
            }

            int expiresIn = int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60");
            var token = _jwtService.GenerateToken(user, expiresIn);

            var userResponse = new UserResponse(
                ID_NguoiDung: user.ID_NguoiDung,
                HoTen: user.HoTen,
                Email: user.Email,
                AnhDaiDien: user.AnhDaiDien,
                VaiTro: user.VaiTro,
                TrangThai: user.TrangThai
            );

            return new AuthResponse(token, expiresIn * 60, userResponse); // ExpiresIn trả về giây
        }
    }
}