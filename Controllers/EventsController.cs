using Buza.Data;
using Buza.Dtos;
using Buza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buza.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPublished()
    {
        var posts = await db.EventPosts
            .AsNoTracking()
            .Include(post => post.Images)
            .Where(post => post.IsPublished)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();

        return Ok(posts.Select(ToDto));
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
