using Buza.Dtos;
using Buza.Models;
using Buza.Repositories;

namespace Buza.Services;

public class ProductService(IProductRepository products, IFileStorageService fileStorage) : IProductService
{
    public async Task<IReadOnlyList<ProductSummaryDto>> GetPublicAsync()
    {
        var items = await products.GetPublicAsync();
        return items.Select(ToSummaryDto).ToList();
    }

    public async Task<IReadOnlyList<ProductDetailsDto>> GetAllForAdminAsync()
    {
        var items = await products.GetAllAsync();
        return items.Select(ToDetailsDto).ToList();
    }

    public async Task<ProductDetailsDto> GetPublicByIdAsync(int id)
    {
        var product = await products.GetByIdAsync(id, activeOnly: true)
            ?? throw new KeyNotFoundException("Товар не найден.");

        return ToDetailsDto(product);
    }

    public async Task<ProductDetailsDto> GetAdminByIdAsync(int id)
    {
        var product = await products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Товар не найден.");

        return ToDetailsDto(product);
    }

    public async Task<ProductDetailsDto> CreateAsync(ProductUpsertDto dto)
    {
        var now = DateTime.UtcNow;
        var product = new Product
        {
            Name = dto.Name.Trim(),
            ShortDescription = dto.ShortDescription.Trim(),
            Description = dto.Description.Trim(),
            Composition = dto.Composition.Trim(),
            Volume = dto.Volume.Trim(),
            Price = dto.Price,
            IsActive = dto.IsActive,
            CreatedAt = now
        };

        await products.AddAsync(product);
        await products.SaveChangesAsync();

        return ToDetailsDto(product);
    }

    public async Task<ProductDetailsDto> UpdateAsync(int id, ProductUpsertDto dto)
    {
        var product = await products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Товар не найден.");

        product.Name = dto.Name.Trim();
        product.ShortDescription = dto.ShortDescription.Trim();
        product.Description = dto.Description.Trim();
        product.Composition = dto.Composition.Trim();
        product.Volume = dto.Volume.Trim();
        product.Price = dto.Price;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await products.SaveChangesAsync();

        return ToDetailsDto(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Товар не найден.");

        foreach (var image in product.Images)
        {
            fileStorage.DeleteFile(image.FilePath);
        }

        products.Remove(product);
        await products.SaveChangesAsync();
    }

    public async Task<ProductImageDto> AddImageAsync(int productId, IFormFile file)
    {
        var product = await products.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException("Товар не найден.");

        var filePath = await fileStorage.SaveImageAsync(file);
        var image = new ProductImage
        {
            ProductId = productId,
            FilePath = filePath,
            CreatedAt = DateTime.UtcNow
        };

        product.Images.Add(image);
        product.UpdatedAt = DateTime.UtcNow;
        await products.SaveChangesAsync();

        return new ProductImageDto(image.Id, image.FilePath, image.CreatedAt);
    }

    public async Task DeleteImageAsync(int productId, int imageId)
    {
        var product = await products.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException("Товар не найден.");
        var image = product.Images.FirstOrDefault(item => item.Id == imageId)
            ?? throw new KeyNotFoundException("Изображение не найдено.");

        fileStorage.DeleteFile(image.FilePath);
        product.Images.Remove(image);
        product.UpdatedAt = DateTime.UtcNow;
        await products.SaveChangesAsync();
    }

    private static ProductSummaryDto ToSummaryDto(Product product) => new(
        product.Id,
        product.Name,
        product.ShortDescription,
        product.Volume,
        product.Price,
        product.Images.OrderBy(image => image.Id).FirstOrDefault()?.FilePath);

    private static ProductDetailsDto ToDetailsDto(Product product) => new(
        product.Id,
        product.Name,
        product.ShortDescription,
        product.Description,
        product.Composition,
        product.Volume,
        product.Price,
        product.IsActive,
        product.CreatedAt,
        product.UpdatedAt,
        product.Images
            .OrderBy(image => image.Id)
            .Select(image => new ProductImageDto(image.Id, image.FilePath, image.CreatedAt))
            .ToList());
}
