using Buza.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buza.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260621120000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AdminUsers",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Login = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                PasswordHash = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                Role = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdminUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                ShortDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: false),
                Composition = table.Column<string>(type: "TEXT", nullable: false),
                Volume = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SiteSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ProjectName = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                LogoPath = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                Phone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                Email = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                Address = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                SocialLinks = table.Column<string>(type: "TEXT", nullable: false),
                FoundationText = table.Column<string>(type: "TEXT", nullable: false),
                FoundationUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                FederalProjectUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                MainImagePath = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                HeroTitle = table.Column<string>(type: "TEXT", maxLength: 220, nullable: false),
                HeroDescription = table.Column<string>(type: "TEXT", nullable: false),
                ProductHistory = table.Column<string>(type: "TEXT", nullable: false),
                ProductDescription = table.Column<string>(type: "TEXT", nullable: false),
                ProductComposition = table.Column<string>(type: "TEXT", nullable: false),
                ProductBenefits = table.Column<string>(type: "TEXT", nullable: false),
                TechnologyText = table.Column<string>(type: "TEXT", nullable: false),
                ProductionSteps = table.Column<string>(type: "TEXT", nullable: false),
                RawMaterials = table.Column<string>(type: "TEXT", nullable: false),
                StartupDescription = table.Column<string>(type: "TEXT", nullable: false),
                ProjectGoals = table.Column<string>(type: "TEXT", nullable: false),
                DevelopmentStages = table.Column<string>(type: "TEXT", nullable: false),
                TeamInfo = table.Column<string>(type: "TEXT", nullable: false),
                DevelopmentPlans = table.Column<string>(type: "TEXT", nullable: false),
                FormSuccessMessage = table.Column<string>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SiteSettings", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FeedbackRequests",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Phone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                Email = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                ProductId = table.Column<int>(type: "INTEGER", nullable: true),
                Message = table.Column<string>(type: "TEXT", nullable: false),
                Status = table.Column<string>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FeedbackRequests", x => x.Id);
                table.ForeignKey(
                    name: "FK_FeedbackRequests_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "ProductImages",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                FilePath = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductImages", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductImages_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AdminUsers_Login",
            table: "AdminUsers",
            column: "Login",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_FeedbackRequests_ProductId",
            table: "FeedbackRequests",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductImages_ProductId",
            table: "ProductImages",
            column: "ProductId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AdminUsers");
        migrationBuilder.DropTable(name: "FeedbackRequests");
        migrationBuilder.DropTable(name: "ProductImages");
        migrationBuilder.DropTable(name: "SiteSettings");
        migrationBuilder.DropTable(name: "Products");
    }
}
