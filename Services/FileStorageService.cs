using Microsoft.Extensions.Options;

namespace Buza.Services;

public class FileStorageService(IWebHostEnvironment environment, IOptions<FileUploadOptions> options) : IFileStorageService
{
    private readonly FileUploadOptions _options = options.Value;

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new ArgumentException("Файл пустой.");
        }

        if (file.Length > _options.MaxSizeBytes)
        {
            throw new ArgumentException($"Размер файла превышает лимит {_options.MaxSizeBytes / 1024 / 1024} МБ.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_options.AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Недопустимое расширение файла.");
        }

        var webRoot = EnsureWebRoot();
        var uploadsPath = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(uploadsPath, fileName);

        await using var stream = File.Create(absolutePath);
        await file.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }

    public void DeleteFile(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var webRoot = EnsureWebRoot();
        var safeRelativePath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var absolutePath = Path.GetFullPath(Path.Combine(webRoot, safeRelativePath));
        var uploadsRoot = Path.GetFullPath(Path.Combine(webRoot, "uploads"));

        if (!absolutePath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }
    }

    private string EnsureWebRoot()
    {
        var webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        Directory.CreateDirectory(webRoot);
        return webRoot;
    }
}
