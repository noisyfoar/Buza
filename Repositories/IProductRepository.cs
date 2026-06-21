using Buza.Models;

namespace Buza.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetPublicAsync();
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id, bool activeOnly = false);
    Task<ProductImage?> GetImageAsync(int imageId);
    Task AddAsync(Product product);
    void Remove(Product product);
    Task SaveChangesAsync();
}
