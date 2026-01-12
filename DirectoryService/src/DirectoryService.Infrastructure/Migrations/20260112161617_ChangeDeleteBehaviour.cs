using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehaviour : Migration
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
                name: "FK_departments_departments_parent_id",
                table: "departments");

            migrationBuilder.AddForeignKey(
                name: "FK_department_location_departments_fk_departmentlocation_depar~",
                table: "department_location",
                column: "fk_departmentlocation_department_id",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_department_location_locations_fk_departmentlocation_locatio~",
                table: "department_location",
                column: "fk_departmentlocation_location_id",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments",
                column: "parent_id",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_department_location_departments_fk_departmentlocation_depar~",
                table: "department_location");

            migrationBuilder.DropForeignKey(
                name: "FK_department_location_locations_fk_departmentlocation_locatio~",
                table: "department_location");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments");

            migrationBuilder.AddForeignKey(
                name: "FK_department_location_departments_fk_departmentlocation_depar~",
                table: "department_location",
                column: "fk_departmentlocation_department_id",
                principalTable: "departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_department_location_locations_fk_departmentlocation_locatio~",
                table: "department_location",
                column: "fk_departmentlocation_location_id",
                principalTable: "locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments",
                column: "parent_id",
                principalTable: "departments",
                principalColumn: "Id");
        }
    }
}
