using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IndexesForNameAndAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_location_name""
                ON locations (name);
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_location_address""
                ON locations ((address));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""locations"".""IX_locations_name"";
            ");

            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""locations"".""IX_locations_address"";
            ");
        }
    }
}
