namespace BracketSmasherBackend.Common
{
    public static class MatchPhases
    {
        public const string WaitingForPlayers = "WaitingForPlayers";
        public const string BothPlayersConnected = "BothPlayersConnected";
        public const string ReadyForCoinFlip = "ReadyForCoinFlip";
        public const string CoinFlipped = "CoinFlipped";
        public const string CoinResult = "CoinResult";
        public const string StageBan = "StageBan";
        public const string InGame = "InGame";
        public const string BetweenGames = "BetweenGames";
        public const string Completed = "Completed";
    }
}
