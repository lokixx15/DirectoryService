using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IndexesForDepIdentifierAndPosName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_department_identifier""
                ON departments (identifier);
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_position_name""
                ON positions (name);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""departments"".""IX_department_identifier"";
            ");

            migrationBuilder.Sql(@"
                DROP INDEX IF EXIST ""positions"".""IX_position_name"";
            ");
        }
    }
}
