namespace BracketSmasherBackend.DTOs;

public class MatchStateDto
{
    public string Phase { get; set; } = "WaitingPlayers";

    public long? CoinWinnerPlayerId { get; set; }

    public long? CurrentTurnPlayerId { get; set; }

    public List<StageStateDto> Stages { get; set; } = new();
}