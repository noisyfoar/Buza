using System.ComponentModel.DataAnnotations;

namespace Buza.Dtos;

public record ProductImageDto(int Id, string FilePath, DateTime CreatedAt);

public record ProductSummaryDto(
    int Id,
    string Name,
    string ShortDescription,
    string Volume,
    decimal Price,
    string? ImagePath);

public record ProductDetailsDto(
    int Id,
    string Name,
    string ShortDescription,
    string Description,
    string Composition,
    string Volume,
    decimal Price,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<ProductImageDto> Images);

public class ProductUpsertDto
{
    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Composition { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Volume { get; set; } = string.Empty;

    [Range(0, 1000000)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
}
