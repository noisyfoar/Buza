using Buza.Data;
using Buza.Models;
using Microsoft.EntityFrameworkCore;

namespace Buza.Repositories;

public class FeedbackRepository(ApplicationDbContext db) : IFeedbackRepository
{
    public Task<List<FeedbackRequest>> GetAllAsync() =>
        db.FeedbackRequests
            .AsNoTracking()
            .Include(request => request.Product)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync();

    public Task<FeedbackRequest?> GetByIdAsync(int id) =>
        db.FeedbackRequests
            .Include(request => request.Product)
            .FirstOrDefaultAsync(request => request.Id == id);

    public async Task AddAsync(FeedbackRequest request) => await db.FeedbackRequests.AddAsync(request);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
