using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BracketSmasherBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Player1Connected",
                table: "MatchSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Player2Connected",
                table: "MatchSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Connected",
                table: "MatchSessions");

            migrationBuilder.DropColumn(
                name: "Player2Connected",
                table: "MatchSessions");
        }
    }
}
