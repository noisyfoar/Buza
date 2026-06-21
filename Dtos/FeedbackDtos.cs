using System.ComponentModel.DataAnnotations;

namespace Buza.Dtos;

public class FeedbackCreateDto
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    public int? ProductId { get; set; }

    [Required, MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "Необходимо согласие на обработку персональных данных.")]
    public bool PersonalDataAccepted { get; set; }
}

public record FeedbackDto(
    int Id,
    string Name,
    string Phone,
    string Email,
    int? ProductId,
    string? ProductName,
    string Message,
    string Status,
    DateTime CreatedAt);

public class FeedbackStatusUpdateDto
{
    [Required]
    public string Status { get; set; } = "NEW";
}
