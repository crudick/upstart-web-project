using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Upstart.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PollAuthenticationRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "requires_authentication",
                table: "polls",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "requires_authentication",
                table: "polls");
        }
    }
}
