using Buza.Models;

namespace Buza.Repositories;

public interface IFeedbackRepository
{
    Task<List<FeedbackRequest>> GetAllAsync();
    Task<FeedbackRequest?> GetByIdAsync(int id);
    Task AddAsync(FeedbackRequest request);
    Task SaveChangesAsync();
}
