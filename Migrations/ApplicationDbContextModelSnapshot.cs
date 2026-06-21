using Buza.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Buza.Migrations;

[DbContext(typeof(ApplicationDbContext))]
public partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "10.0.9");

        modelBuilder.Entity("Buza.Models.AdminUser", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<string>("Login").IsRequired().HasMaxLength(64).HasColumnType("TEXT");
            b.Property<string>("PasswordHash").IsRequired().HasMaxLength(300).HasColumnType("TEXT");
            b.Property<string>("Role").IsRequired().HasMaxLength(40).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("Login").IsUnique();
            b.ToTable("AdminUsers");
        });

        modelBuilder.Entity("Buza.Models.FeedbackRequest", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<string>("Email").IsRequired().HasMaxLength(160).HasColumnType("TEXT");
            b.Property<string>("Message").IsRequired().HasColumnType("TEXT");
            b.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
            b.Property<string>("Phone").IsRequired().HasMaxLength(40).HasColumnType("TEXT");
            b.Property<int?>("ProductId").HasColumnType("INTEGER");
            b.Property<string>("Status").IsRequired().HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("ProductId");
            b.ToTable("FeedbackRequests");
        });

        modelBuilder.Entity("Buza.Models.Product", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<string>("Composition").IsRequired().HasColumnType("TEXT");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<string>("Description").IsRequired().HasColumnType("TEXT");
            b.Property<bool>("IsActive").HasColumnType("INTEGER");
            b.Property<string>("Name").IsRequired().HasMaxLength(160).HasColumnType("TEXT");
            b.Property<decimal>("Price").HasColumnType("decimal(18,2)");
            b.Property<string>("ShortDescription").IsRequired().HasMaxLength(500).HasColumnType("TEXT");
            b.Property<DateTime?>("UpdatedAt").HasColumnType("TEXT");
            b.Property<string>("Volume").IsRequired().HasMaxLength(80).HasColumnType("TEXT");
            b.HasKey("Id");
            b.ToTable("Products");
        });

        modelBuilder.Entity("Buza.Models.ProductImage", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<string>("FilePath").IsRequired().HasMaxLength(300).HasColumnType("TEXT");
            b.Property<int>("ProductId").HasColumnType("INTEGER");
            b.HasKey("Id");
            b.HasIndex("ProductId");
            b.ToTable("ProductImages");
        });

        modelBuilder.Entity("Buza.Models.SiteSettings", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<string>("Address").IsRequired().HasMaxLength(300).HasColumnType("TEXT");
            b.Property<string>("DevelopmentPlans").IsRequired().HasColumnType("TEXT");
            b.Property<string>("DevelopmentStages").IsRequired().HasColumnType("TEXT");
            b.Property<string>("Email").IsRequired().HasMaxLength(160).HasColumnType("TEXT");
            b.Property<string>("FederalProjectUrl").IsRequired().HasMaxLength(300).HasColumnType("TEXT");
            b.Property<string>("FormSuccessMessage").IsRequired().HasColumnType("TEXT");
            b.Property<string>("FoundationText").IsRequired().HasColumnType("TEXT");
            b.Property<string>("FoundationUrl").IsRequired().HasMaxLength(300).HasColumnType("TEXT");
            b.Property<string>("HeroDescription").IsRequired().HasColumnType("TEXT");
            b.Property<string>("HeroTitle").IsRequired().HasMaxLength(220).HasColumnType("TEXT");
            b.Property<string>("LogoPath").IsRequired().HasMaxLength(300).HasColumnType("TEXT");
            b.Property<string>("MainImagePath").IsRequired().HasMaxLength(300).HasColumnType("TEXT");
            b.Property<string>("Phone").IsRequired().HasMaxLength(40).HasColumnType("TEXT");
            b.Property<string>("ProductBenefits").IsRequired().HasColumnType("TEXT");
            b.Property<string>("ProductComposition").IsRequired().HasColumnType("TEXT");
            b.Property<string>("ProductDescription").IsRequired().HasColumnType("TEXT");
            b.Property<string>("ProductHistory").IsRequired().HasColumnType("TEXT");
            b.Property<string>("ProductionSteps").IsRequired().HasColumnType("TEXT");
            b.Property<string>("ProjectGoals").IsRequired().HasColumnType("TEXT");
            b.Property<string>("ProjectName").IsRequired().HasMaxLength(160).HasColumnType("TEXT");
            b.Property<string>("RawMaterials").IsRequired().HasColumnType("TEXT");
            b.Property<string>("SocialLinks").IsRequired().HasColumnType("TEXT");
            b.Property<string>("StartupDescription").IsRequired().HasColumnType("TEXT");
            b.Property<string>("TeamInfo").IsRequired().HasColumnType("TEXT");
            b.Property<string>("TechnologyText").IsRequired().HasColumnType("TEXT");
            b.Property<DateTime>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.ToTable("SiteSettings");
        });

        modelBuilder.Entity("Buza.Models.FeedbackRequest", b =>
        {
            b.HasOne("Buza.Models.Product", "Product")
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.SetNull);
            b.Navigation("Product");
        });

        modelBuilder.Entity("Buza.Models.ProductImage", b =>
        {
            b.HasOne("Buza.Models.Product", "Product")
                .WithMany("Images")
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Product");
        });

        modelBuilder.Entity("Buza.Models.Product", b =>
        {
            b.Navigation("Images");
        });
#pragma warning restore 612, 618
    }
}
