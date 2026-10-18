using Buza.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buza.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261018120000_RemoveFeedbackRequests")]
public partial class RemoveFeedbackRequests : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"FeedbackRequests\";");
        migrationBuilder.Sql("ALTER TABLE \"SiteSettings\" DROP COLUMN \"FormSuccessMessage\";");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "FormSuccessMessage",
            table: "SiteSettings",
            type: "TEXT",
            nullable: false,
            defaultValue: "");

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

        migrationBuilder.CreateIndex(
            name: "IX_FeedbackRequests_ProductId",
            table: "FeedbackRequests",
            column: "ProductId");
    }
}
