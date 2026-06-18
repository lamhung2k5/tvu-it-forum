using System.Text.RegularExpressions;
using ForumAPI.DTOs.Auth;
using ForumAPI.Models;
using ForumAPI.Repositories;
using Microsoft.Extensions.Configuration;

namespace ForumAPI.Services
{
    public class AuthService : IAuthService
    {
        // Regex email tổng quát dùng cho đăng nhập.
        // Cho phép admin đăng nhập bằng Gmail, ví dụ: admin.tvu.forum@gmail.com
        // Chặn các lỗi như: email@gmail,com, email@gmail.com., email..abc@gmail.com
        private static readonly Regex EmailRegex = new(
            @"^[a-z0-9]+(?:[._%+-][a-z0-9]+)*@(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\.)+[a-z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Regex riêng cho đăng ký sinh viên TVU.
        // Định dạng bắt buộc: 10 chữ số + @st.tvu.edu.vn
        // Ví dụ hợp lệ: 1101230015@st.tvu.edu.vn
        private static readonly Regex TvuStudentEmailRegex = new(
            @"^\d{10}@st\.tvu\.edu\.vn$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

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
            var hoTen = request.HoTen?.Trim() ?? string.Empty;
            var email = NormalizeEmail(request.Email);
            var password = request.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                throw new Exception("Vui lòng nhập họ tên.");
            }

            // Đăng ký chỉ cho phép email sinh viên TVU.
            // Admin Gmail vẫn đăng nhập được vì admin đã có trong dữ liệu mẫu.
            if (!IsValidTvuStudentEmail(email))
            {
                throw new Exception("Email sinh viên không hợp lệ. Định dạng đúng: 1101230015@st.tvu.edu.vn");
            }

            if (password.Length < 6)
            {
                throw new Exception("Mật khẩu phải có tối thiểu 6 ký tự.");
            }

            if (await _userRepo.EmailExistsAsync(email))
            {
                throw new Exception("Email đã tồn tại trong hệ thống.");
            }

            var user = new NguoiDung
            {
                HoTen = hoTen,
                Email = email,
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(password)
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
            var email = NormalizeEmail(request.Email);
            var password = request.Password ?? string.Empty;

            // Đăng nhập dùng regex email tổng quát để admin Gmail vẫn đăng nhập được.
            if (!IsValidEmail(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");
            }

            var user = await _userRepo.GetByEmailAsync(email)
                ?? throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.MatKhauHash))
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

            return new AuthResponse(token, expiresIn * 60, userResponse);
        }

        private static string NormalizeEmail(string? email)
        {
            return (email ?? string.Empty).Trim().ToLowerInvariant();
        }

        private static bool IsValidEmail(string email)
        {
            return EmailRegex.IsMatch(email);
        }

        private static bool IsValidTvuStudentEmail(string email)
        {
            return TvuStudentEmailRegex.IsMatch(email);
        }
    }
}