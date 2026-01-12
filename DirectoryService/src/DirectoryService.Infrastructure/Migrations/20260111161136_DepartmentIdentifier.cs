using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DepartmentIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_departments_DepartmentId",
                table: "departments",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_departments_DepartmentId",
                table: "departments",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_departments_DepartmentId",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "IX_departments_DepartmentId",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "departments");
        }
    }
}
