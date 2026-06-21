using System.ComponentModel.DataAnnotations;

namespace Buza.Models;

public class FeedbackRequest
{
    public int Id { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    public int? ProductId { get; set; }

    public string Message { get; set; } = string.Empty;

    public FeedbackStatus Status { get; set; } = FeedbackStatus.NEW;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
}
