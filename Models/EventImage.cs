using System.ComponentModel.DataAnnotations;

namespace Buza.Models;

public class EventImage
{
    public int Id { get; set; }

    public int EventPostId { get; set; }

    [MaxLength(300)]
    public string FilePath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EventPost EventPost { get; set; } = null!;
}
