using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "media_asset",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_type = table.Column<string>(type: "text", nullable: false),
                    media_status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    raw_key = table.Column<string>(type: "jsonb", nullable: false),
                    final_key = table.Column<string>(type: "jsonb", nullable: false),
                    media_type = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    hsl_root_key = table.Column<string>(type: "jsonb", nullable: true),
                    media_data = table.Column<string>(type: "jsonb", nullable: false),
                    media_owner = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_asset", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "media_asset");
        }
    }
}
