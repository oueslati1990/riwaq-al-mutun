using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatnMemorization.Infrastructure.Persistence.Migrations;

public partial class AddMatnEntities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Matns",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Author = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                Status = table.Column<int>(type: "INTEGER", nullable: false),
                IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Matns", x => x.Id));

        migrationBuilder.CreateTable(
            name: "MatnSections",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                MatnId = table.Column<Guid>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Order = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MatnSections", x => x.Id);
                table.ForeignKey(
                    name: "FK_MatnSections_Matns_MatnId",
                    column: x => x.MatnId,
                    principalTable: "Matns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TextUnits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                SectionId = table.Column<Guid>(type: "TEXT", nullable: false),
                Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                SearchIndex = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                Order = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TextUnits", x => x.Id);
                table.ForeignKey(
                    name: "FK_TextUnits_MatnSections_SectionId",
                    column: x => x.SectionId,
                    principalTable: "MatnSections",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Matns_IsDeleted", table: "Matns", column: "IsDeleted");
        migrationBuilder.CreateIndex(name: "IX_Matns_Category", table: "Matns", column: "Category");
        migrationBuilder.CreateIndex(name: "IX_MatnSections_MatnId_Order", table: "MatnSections", columns: new[] { "MatnId", "Order" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_TextUnits_SectionId_Order", table: "TextUnits", columns: new[] { "SectionId", "Order" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_TextUnits_SearchIndex", table: "TextUnits", column: "SearchIndex");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "TextUnits");
        migrationBuilder.DropTable(name: "MatnSections");
        migrationBuilder.DropTable(name: "Matns");
    }
}
