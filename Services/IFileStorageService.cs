namespace Buza.Services;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(IFormFile file);
    void DeleteFile(string relativePath);
}
