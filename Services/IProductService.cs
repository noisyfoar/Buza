using Buza.Dtos;

namespace Buza.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductSummaryDto>> GetPublicAsync();
    Task<IReadOnlyList<ProductDetailsDto>> GetAllForAdminAsync();
    Task<ProductDetailsDto> GetPublicByIdAsync(int id);
    Task<ProductDetailsDto> GetAdminByIdAsync(int id);
    Task<ProductDetailsDto> CreateAsync(ProductUpsertDto dto);
    Task<ProductDetailsDto> UpdateAsync(int id, ProductUpsertDto dto);
    Task DeleteAsync(int id);
    Task<ProductImageDto> AddImageAsync(int productId, IFormFile file);
    Task DeleteImageAsync(int productId, int imageId);
}
