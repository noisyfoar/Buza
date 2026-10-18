using Buza.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buza.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261018130000_AddEventPosts")]
public partial class AddEventPosts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventPosts",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Content = table.Column<string>(type: "TEXT", nullable: false),
                IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_EventPosts", post => post.Id));

        migrationBuilder.CreateTable(
            name: "EventImages",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                EventPostId = table.Column<int>(type: "INTEGER", nullable: false),
                FilePath = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventImages", image => image.Id);
                table.ForeignKey(
                    name: "FK_EventImages_EventPosts_EventPostId",
                    column: image => image.EventPostId,
                    principalTable: "EventPosts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_EventImages_EventPostId",
            table: "EventImages",
            column: "EventPostId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EventImages");
        migrationBuilder.DropTable(name: "EventPosts");
    }
}
