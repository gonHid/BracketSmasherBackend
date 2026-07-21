using System.ComponentModel.DataAnnotations;

namespace BracketSmasherBackend.Models;

public class StageState
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MatchSessionId { get; set; }

    public MatchSession MatchSession { get; set; } = null!;

    public int StageId { get; set; }

    // "neutral", "banned", "selected"
    public string State { get; set; } = "neutral";

    public long UpdatedByPlayerId { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}