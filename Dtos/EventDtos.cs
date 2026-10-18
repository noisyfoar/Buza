using System.ComponentModel.DataAnnotations;

namespace Buza.Dtos;

public record EventImageDto(int Id, string FilePath, DateTime CreatedAt);

public record EventPostDto(
    int Id,
    string Title,
    string Content,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<EventImageDto> Images);

public class EventPostUpsertDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsPublished { get; set; } = true;
}
