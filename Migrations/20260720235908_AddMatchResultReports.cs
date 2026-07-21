using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BracketSmasherBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchResultReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchResultReports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetId = table.Column<long>(type: "bigint", nullable: false),
                    MatchSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReporterPlayerId = table.Column<long>(type: "bigint", nullable: false),
                    WinnerPlayerId = table.Column<long>(type: "bigint", nullable: false),
                    WinnerTag = table.Column<string>(type: "text", nullable: false),
                    LoserPlayerId = table.Column<long>(type: "bigint", nullable: false),
                    LoserTag = table.Column<string>(type: "text", nullable: false),
                    WinnerScore = table.Column<int>(type: "integer", nullable: false),
                    LoserScore = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResultReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchResultReports_MatchSessions_MatchSessionId",
                        column: x => x.MatchSessionId,
                        principalTable: "MatchSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchResultReports_MatchSessionId",
                table: "MatchResultReports",
                column: "MatchSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResultReports_SetId_ReporterPlayerId",
                table: "MatchResultReports",
                columns: new[] { "SetId", "ReporterPlayerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchResultReports");
        }
    }
}
