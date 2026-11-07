using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatnMemorization.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddMemorizationProgress : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MemorizationProgresses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                TextUnitId = table.Column<Guid>(type: "TEXT", nullable: false),
                EaseFactor = table.Column<double>(type: "REAL", nullable: false, defaultValue: 2.5),
                Interval = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                ReviewCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                NextReviewDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                LastReviewedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemorizationProgresses", x => x.Id);
                table.ForeignKey(
                    name: "FK_MemorizationProgresses_TextUnits_TextUnitId",
                    column: x => x.TextUnitId,
                    principalTable: "TextUnits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ReviewAttempts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                MemorizationProgressId = table.Column<Guid>(type: "TEXT", nullable: false),
                Quality = table.Column<int>(type: "INTEGER", nullable: false),
                ReviewedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReviewAttempts", x => x.Id);
                table.ForeignKey(
                    name: "FK_ReviewAttempts_MemorizationProgresses_MemorizationProgressId",
                    column: x => x.MemorizationProgressId,
                    principalTable: "MemorizationProgresses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_MemorizationProgresses_TextUnitId",
            table: "MemorizationProgresses",
            column: "TextUnitId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MemorizationProgresses_NextReviewDate",
            table: "MemorizationProgresses",
            column: "NextReviewDate");

        migrationBuilder.CreateIndex(
            name: "IX_ReviewAttempts_MemorizationProgressId",
            table: "ReviewAttempts",
            column: "MemorizationProgressId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ReviewAttempts");
        migrationBuilder.DropTable(name: "MemorizationProgresses");
    }
}
