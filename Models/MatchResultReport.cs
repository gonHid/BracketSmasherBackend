using Microsoft.EntityFrameworkCore;

namespace BracketSmasherBackend.Models;

[Index(nameof(SetId), nameof(ReporterPlayerId), IsUnique = true)]
public class MatchResultReport
{
    public long Id { get; set; }

    public string SetId { get; set; } = string.Empty;
    public Guid MatchSessionId { get; set; }

    public MatchSession MatchSession { get; set; } = null!;

    public long ReporterPlayerId { get; set; }

    public long WinnerPlayerId { get; set; }
    public string WinnerTag { get; set; } = "";

    public long LoserPlayerId { get; set; }
    public string LoserTag { get; set; } = "";

    public int WinnerScore { get; set; }
    public int LoserScore { get; set; }

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;
}