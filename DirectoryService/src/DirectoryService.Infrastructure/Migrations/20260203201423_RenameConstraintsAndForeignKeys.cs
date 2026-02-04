using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameConstraintsAndForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_department_location_departments_fk_departmentlocation_depar~",
                table: "department_location");

            migrationBuilder.DropForeignKey(
                name: "FK_department_location_locations_fk_departmentlocation_locatio~",
                table: "department_location");

            migrationBuilder.DropForeignKey(
                name: "FK_department_position_departments_fk_departmentposition_depar~",
                table: "department_position");

            migrationBuilder.DropForeignKey(
                name: "FK_department_position_positions_fk_departmentposition_positio~",
                table: "department_position");

            migrationBuilder.RenameColumn(
                name: "fk_departmentposition_position_id",
                table: "department_position",
                newName: "position_id");

            migrationBuilder.RenameColumn(
                name: "fk_departmentposition_department_id",
                table: "department_position",
                newName: "department_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_position_fk_departmentposition_position_id",
                table: "department_position",
                newName: "IX_department_position_position_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_position_fk_departmentposition_department_id",
                table: "department_position",
                newName: "IX_department_position_department_id");

            migrationBuilder.RenameColumn(
                name: "fk_departmentlocation_location_id",
                table: "department_location",
                newName: "location_id");

            migrationBuilder.RenameColumn(
                name: "fk_departmentlocation_department_id",
                table: "department_location",
                newName: "department_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_location_fk_departmentlocation_location_id",
                table: "department_location",
                newName: "IX_department_location_location_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_location_fk_departmentlocation_department_id",
                table: "department_location",
                newName: "IX_department_location_department_id");

            migrationBuilder.AddForeignKey(
                name: "fk_departmentlocation_department_id",
                table: "department_location",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_departmentlocation_location_id",
                table: "department_location",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_departmentposition_department_id",
                table: "department_position",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_departmentposition_position_id",
                table: "department_position",
                column: "position_id",
                principalTable: "positions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_departmentlocation_department_id",
                table: "department_location");

            migrationBuilder.DropForeignKey(
                name: "fk_departmentlocation_location_id",
                table: "department_location");

            migrationBuilder.DropForeignKey(
                name: "fk_departmentposition_department_id",
                table: "department_position");

            migrationBuilder.DropForeignKey(
                name: "fk_departmentposition_position_id",
                table: "department_position");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "department_position",
                newName: "fk_departmentposition_position_id");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "department_position",
                newName: "fk_departmentposition_department_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_position_position_id",
                table: "department_position",
                newName: "IX_department_position_fk_departmentposition_position_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_position_department_id",
                table: "department_position",
                newName: "IX_department_position_fk_departmentposition_department_id");

            migrationBuilder.RenameColumn(
                name: "location_id",
                table: "department_location",
                newName: "fk_departmentlocation_location_id");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "department_location",
                newName: "fk_departmentlocation_department_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_location_location_id",
                table: "department_location",
                newName: "IX_department_location_fk_departmentlocation_location_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_location_department_id",
                table: "department_location",
                newName: "IX_department_location_fk_departmentlocation_department_id");

            migrationBuilder.AddForeignKey(
                name: "FK_department_location_departments_fk_departmentlocation_depar~",
                table: "department_location",
                column: "fk_departmentlocation_department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_department_location_locations_fk_departmentlocation_locatio~",
                table: "department_location",
                column: "fk_departmentlocation_location_id",
                principalTable: "locations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_department_position_departments_fk_departmentposition_depar~",
                table: "department_position",
                column: "fk_departmentposition_department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_department_position_positions_fk_departmentposition_positio~",
                table: "department_position",
                column: "fk_departmentposition_position_id",
                principalTable: "positions",
                principalColumn: "id");
        }
    }
}
