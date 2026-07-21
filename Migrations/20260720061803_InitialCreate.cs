using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BracketSmasherBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SetId = table.Column<long>(type: "bigint", nullable: false),
                    Player1Id = table.Column<long>(type: "bigint", nullable: false),
                    Player2Id = table.Column<long>(type: "bigint", nullable: false),
                    Phase = table.Column<string>(type: "text", nullable: false),
                    CoinWinnerPlayerId = table.Column<long>(type: "bigint", nullable: true),
                    CurrentTurnPlayerId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSessions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchSessions");
        }
    }
}
