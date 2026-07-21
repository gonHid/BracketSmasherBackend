namespace BracketSmasherBackend.Models;

public class StageBan
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MatchSessionId { get; set; }

    public MatchSession MatchSession { get; set; } = null!;

    public int StageId { get; set; }

    public int BanOrder { get; set; }

    public long BannedByPlayerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}