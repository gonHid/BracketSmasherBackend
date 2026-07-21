namespace BracketSmasherBackend.Models;

public class TournamentSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // ID de Start.gg
    public long TournamentId { get; set; }

    public string Name { get; set; } = string.Empty;

    // Fecha oficial del torneo
    public DateTime StartAt { get; set; }

    // Expira 7 días después del inicio
    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<MatchSession> Matches { get; set; } = new();
}