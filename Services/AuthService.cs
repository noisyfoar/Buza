using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Buza.Dtos;
using Buza.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Buza.Services;

public class AuthService(
    IAdminUserRepository admins,
    IPasswordHasher passwordHasher,
    IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var admin = await admins.GetByLoginAsync(dto.Login.Trim());
        if (admin is null || !passwordHasher.Verify(dto.Password, admin.PasswordHash))
        {
            throw new UnauthorizedAccessException("Неверный логин или пароль.");
        }

        var jwtSection = configuration.GetSection("Jwt");
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtSection.GetValue("ExpiresMinutes", 720));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new Claim(ClaimTypes.Name, admin.Login),
            new Claim(ClaimTypes.Role, admin.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            admin.Login,
            admin.Role);
    }
}
