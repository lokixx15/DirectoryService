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

            migrationBuilder.CreateTable(
                name: "video_processes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    raw_key = table.Column<string>(type: "jsonb", nullable: false),
                    hls_key = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    current_step_order = table.Column<int>(type: "integer", nullable: true),
                    current_step_type = table.Column<string>(type: "text", nullable: true),
                    current_step_progress = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    total_progress = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    metadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_video_process_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "video_process_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    process_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    step_type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    progress = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_video_process_step_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_video_process_steps_video_processes_process_id",
                        column: x => x.process_id,
                        principalTable: "video_processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_video_process_steps_process_id",
                table: "video_process_steps",
                column: "process_id");

            migrationBuilder.CreateIndex(
                name: "ix_video_process_created_at",
                table: "video_processes",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_video_process_status",
                table: "video_processes",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "media_asset");

            migrationBuilder.DropTable(
                name: "video_process_steps");

            migrationBuilder.DropTable(
                name: "video_processes");
        }
    }
}
