using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BracketSmasherBackend.Models;

[Index(nameof(SetId), IsUnique = true)]
public class MatchSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // IDs de Start.gg
    public long TournamentId { get; set; }

    public long EventId { get; set; }

    public string SetId { get; set; } = string.Empty;

    // Jugadores
    public long Player1Id { get; set; }

    public long Player2Id { get; set; }

    // Conexión
    public bool Player1Connected { get; set; }

    public bool Player2Connected { get; set; }

    // Estado de la sala
    public string Phase { get; set; } = "WaitingPlayers";

    public long? CoinWinnerPlayerId { get; set; }

    public long? CurrentTurnPlayerId { get; set; }

    // Fechas
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Expira 7 días después del inicio del torneo
    public DateTime ExpiresAt { get; set; }

    // Navegación
    public List<StageBan> StageBans { get; set; } = new();
    public List<MatchResultReport> ResultReports { get; set; } = new();
}