using Buza.Data;
using Buza.Models;
using Microsoft.EntityFrameworkCore;

namespace Buza.Repositories;

public class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    public Task<List<Product>> GetPublicAsync() =>
        db.Products
            .AsNoTracking()
            .Include(product => product.Images)
            .Where(product => product.IsActive)
            .OrderByDescending(product => product.CreatedAt)
            .ToListAsync();

    public Task<List<Product>> GetAllAsync() =>
        db.Products
            .AsNoTracking()
            .Include(product => product.Images)
            .OrderByDescending(product => product.CreatedAt)
            .ToListAsync();

    public Task<Product?> GetByIdAsync(int id, bool activeOnly = false)
    {
        var query = db.Products.Include(product => product.Images).Where(product => product.Id == id);
        if (activeOnly)
        {
            query = query.Where(product => product.IsActive);
        }

        return query.FirstOrDefaultAsync();
    }

    public Task<ProductImage?> GetImageAsync(int imageId) =>
        db.ProductImages.FirstOrDefaultAsync(image => image.Id == imageId);

    public async Task AddAsync(Product product) => await db.Products.AddAsync(product);

    public void Remove(Product product) => db.Products.Remove(product);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
