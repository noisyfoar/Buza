using Buza.Models;
using Microsoft.EntityFrameworkCore;

namespace Buza.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<FeedbackRequest> FeedbackRequests => Set<FeedbackRequest>();

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Price).HasColumnType("decimal(18,2)");
            entity.HasMany(product => product.Images)
                .WithOne(image => image.Product)
                .HasForeignKey(image => image.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FeedbackRequest>(entity =>
        {
            entity.Property(request => request.Status).HasConversion<string>();
            entity.HasOne(request => request.Product)
                .WithMany()
                .HasForeignKey(request => request.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AdminUser>()
            .HasIndex(admin => admin.Login)
            .IsUnique();
    }
}
