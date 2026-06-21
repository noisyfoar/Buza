using Buza.Dtos;
using Buza.Models;
using Buza.Repositories;

namespace Buza.Services;

public class FeedbackService(IFeedbackRepository feedback, IProductRepository products) : IFeedbackService
{
    public async Task<FeedbackDto> CreateAsync(FeedbackCreateDto dto)
    {
        if (!dto.PersonalDataAccepted)
        {
            throw new ArgumentException("Необходимо согласие на обработку персональных данных.");
        }

        if (dto.ProductId.HasValue && await products.GetByIdAsync(dto.ProductId.Value, activeOnly: true) is null)
        {
            throw new ArgumentException("Выбранный товар не найден.");
        }

        var request = new FeedbackRequest
        {
            Name = dto.Name.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email.Trim(),
            ProductId = dto.ProductId,
            Message = dto.Message.Trim(),
            Status = FeedbackStatus.NEW,
            CreatedAt = DateTime.UtcNow
        };

        await feedback.AddAsync(request);
        await feedback.SaveChangesAsync();

        return ToDto(request);
    }

    public async Task<IReadOnlyList<FeedbackDto>> GetAllAsync()
    {
        var requests = await feedback.GetAllAsync();
        return requests.Select(ToDto).ToList();
    }

    public async Task<FeedbackDto> GetByIdAsync(int id)
    {
        var request = await feedback.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Заявка не найдена.");

        return ToDto(request);
    }

    public async Task<FeedbackDto> UpdateStatusAsync(int id, string status)
    {
        var request = await feedback.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Заявка не найдена.");

        if (!Enum.TryParse<FeedbackStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            throw new ArgumentException("Недопустимый статус заявки.");
        }

        request.Status = parsedStatus;
        await feedback.SaveChangesAsync();

        return ToDto(request);
    }

    private static FeedbackDto ToDto(FeedbackRequest request) => new(
        request.Id,
        request.Name,
        request.Phone,
        request.Email,
        request.ProductId,
        request.Product?.Name,
        request.Message,
        request.Status.ToString(),
        request.CreatedAt);
}
