using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPositionIndexCreatedAtId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_position_id_created_at",
                table: "positions");

            migrationBuilder.CreateIndex(
                name: "idx_positions_created_at_id",
                table: "positions",
                columns: new[] { "created_at", "id" },
                descending: Array.Empty<bool>());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_positions_created_at_id",
                table: "positions");

            migrationBuilder.CreateIndex(
                name: "idx_position_id_created_at",
                table: "positions",
                columns: new[] { "id", "created_at" });
        }
    }
}
