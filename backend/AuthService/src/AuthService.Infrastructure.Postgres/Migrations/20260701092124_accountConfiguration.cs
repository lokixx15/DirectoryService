using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class accountConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                schema: "auth",
                table: "accounts",
                newName: "normalized_user_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "normalized_user_name",
                schema: "auth",
                table: "accounts",
                newName: "NormalizedUserName");
        }
    }
}
