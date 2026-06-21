using System.ComponentModel.DataAnnotations;

namespace Buza.Models;

public class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    [MaxLength(300)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
}
