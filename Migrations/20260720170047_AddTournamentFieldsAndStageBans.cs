using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BracketSmasherBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentFieldsAndStageBans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "MatchSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TournamentId",
                table: "MatchSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "StageBans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StageId = table.Column<int>(type: "integer", nullable: false),
                    BanOrder = table.Column<int>(type: "integer", nullable: false),
                    BannedByPlayerId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageBans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageBans_MatchSessions_MatchSessionId",
                        column: x => x.MatchSessionId,
                        principalTable: "MatchSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StageBans_MatchSessionId",
                table: "StageBans",
                column: "MatchSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StageBans");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "MatchSessions");

            migrationBuilder.DropColumn(
                name: "TournamentId",
                table: "MatchSessions");
        }
    }
}
