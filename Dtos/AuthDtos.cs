using System.ComponentModel.DataAnnotations;

namespace Buza.Dtos;

public class LoginRequestDto
{
    [Required]
    public string Login { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record AuthResponseDto(string Token, DateTime ExpiresAt, string Login, string Role);
