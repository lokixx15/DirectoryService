using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DepartmentPathToLtree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS ltree;");

            migrationBuilder.Sql(@"
                ALTER TABLE departments 
                ALTER COLUMN path 
                TYPE ltree 
                USING path::ltree;
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX idx_departments_path 
                ON departments USING GIST (path);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""departments"".""idx_departments_path"";
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE departments 
                ALTER COLUMN path 
                TYPE character varying(300);
            ");
        }
    }
}
