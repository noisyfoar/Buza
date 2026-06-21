using System.ComponentModel.DataAnnotations;

namespace Buza.Models;

public class AdminUser
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string Login { get; set; } = string.Empty;

    [MaxLength(300)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Role { get; set; } = "Admin";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
