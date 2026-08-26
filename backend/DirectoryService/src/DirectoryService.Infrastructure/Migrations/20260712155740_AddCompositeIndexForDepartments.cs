using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeIndexForDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_departments_parent_id_is_active",
                table: "departments",
                columns: new[] { "parent_id", "is_active" });

            migrationBuilder.Sql("CREATE INDEX IX_departments_path_gist ON departments USING GIST (path);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_departments_parent_id_is_active",
                table: "departments");

            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_departments_path_gist;");
        }
    }
}
