using Buza.Data;
using Buza.Dtos;
using Buza.Models;
using Buza.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buza.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/events")]
public class AdminEventsController(ApplicationDbContext db, IFileStorageService fileStorage) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await db.EventPosts.AsNoTracking()
            .Include(post => post.Images)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();
        return Ok(posts.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventPostUpsertDto dto)
    {
        var post = new EventPost
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        db.EventPosts.Add(post);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), ToDto(post));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventPostUpsertDto dto)
    {
        var post = await db.EventPosts.Include(item => item.Images).FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new KeyNotFoundException("Публикация не найдена.");

        post.Title = dto.Title.Trim();
        post.Content = dto.Content.Trim();
        post.IsPublished = dto.IsPublished;
        post.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(ToDto(post));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await db.EventPosts.Include(item => item.Images).FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new KeyNotFoundException("Публикация не найдена.");

        foreach (var image in post.Images)
        {
            fileStorage.DeleteFile(image.FilePath);
        }

        db.EventPosts.Remove(post);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/images")]
    [RequestSizeLimit(52_428_800)]
    public async Task<IActionResult> AddImages(int id, [FromForm] List<IFormFile> files)
    {
        var post = await db.EventPosts.Include(item => item.Images).FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new KeyNotFoundException("Публикация не найдена.");

        if (files.Count == 0 || files.Count > 10)
        {
            return BadRequest(new { error = "Выберите от 1 до 10 изображений за одну загрузку." });
        }

        var savedPaths = new List<string>();
        try
        {
            foreach (var file in files)
            {
                var path = await fileStorage.SaveImageAsync(file);
                savedPaths.Add(path);
                post.Images.Add(new EventImage { FilePath = path, CreatedAt = DateTime.UtcNow });
            }

            post.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
        catch
        {
            foreach (var path in savedPaths)
            {
                fileStorage.DeleteFile(path);
            }

            throw;
        }

        return Ok(post.Images.OrderBy(image => image.Id)
            .Select(image => new EventImageDto(image.Id, image.FilePath, image.CreatedAt)));
    }

    [HttpDelete("{postId:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int postId, int imageId)
    {
        var image = await db.EventImages.FirstOrDefaultAsync(item => item.EventPostId == postId && item.Id == imageId)
            ?? throw new KeyNotFoundException("Изображение не найдено.");

        fileStorage.DeleteFile(image.FilePath);
        db.EventImages.Remove(image);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static EventPostDto ToDto(EventPost post) => new(
        post.Id,
        post.Title,
        post.Content,
        post.IsPublished,
        post.CreatedAt,
        post.UpdatedAt,
        post.Images.OrderBy(image => image.Id)
            .Select(image => new EventImageDto(image.Id, image.FilePath, image.CreatedAt))
            .ToList());
}
