using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BracketSmasherBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToSetId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchSessions_SetId",
                table: "MatchSessions",
                column: "SetId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchSessions_SetId",
                table: "MatchSessions");
        }
    }
}
