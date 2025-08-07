using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Upstart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class statsessionid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "session_id",
                table: "poll_stats",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "session_id",
                table: "poll_stats");
        }
    }
}
