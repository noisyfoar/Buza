namespace Buza.Services;

public class FileUploadOptions
{
    public long MaxSizeBytes { get; set; } = 5 * 1024 * 1024;

    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
}
