using Buza.Dtos;

namespace Buza.Services;

public interface IFeedbackService
{
    Task<FeedbackDto> CreateAsync(FeedbackCreateDto dto);
    Task<IReadOnlyList<FeedbackDto>> GetAllAsync();
    Task<FeedbackDto> GetByIdAsync(int id);
    Task<FeedbackDto> UpdateStatusAsync(int id, string status);
}
