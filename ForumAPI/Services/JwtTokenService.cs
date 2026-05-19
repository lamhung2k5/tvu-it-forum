using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ForumAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace ForumAPI.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(NguoiDung user, int expiresInMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID_NguoiDung.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("HoTen", user.HoTen),
                new Claim(ClaimTypes.Role, user.VaiTro)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}   