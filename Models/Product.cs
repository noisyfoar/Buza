using System.ComponentModel.DataAnnotations;

namespace Buza.Models;

public class Product
{
    public int Id { get; set; }

    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Composition { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Volume { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public List<ProductImage> Images { get; set; } = [];
}
