using System.Collections.Concurrent;
using BracketSmasherBackend.Common;
using BracketSmasherBackend.Models;
using BracketSmasherBackend.Services;
using Microsoft.AspNetCore.SignalR;

namespace BracketSmasherBackend.Hubs;

public class MatchHub : Hub
{
    private readonly MatchService _matches;
    private readonly StartGgService _startGg;
    private readonly EmailService _email;

    private static readonly ConcurrentDictionary<long, List<MatchResultReport>> _pendingReports = new();
    private static readonly ConcurrentDictionary<long, DateTime> _lastConflictNotification = new();

    public MatchHub(
        MatchService matches,
        StartGgService startGg,
        EmailService email)
    {
        _matches = matches;
        _startGg = startGg;
        _email = email;
    }

    public async Task JoinMatch(
        long tournamentId,
        long eventId,
        long setId,
        long tournamentStartAt,
        long playerId,
        long player1Id,
        long player2Id)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            setId.ToString()
        );

        var tournamentStartDate =
            DateTimeOffset.FromUnixTimeSeconds(tournamentStartAt)
            .UtcDateTime;

        var match = await _matches.GetOrCreateAsync(
            tournamentId,
            eventId,
            setId,
            tournamentStartDate,
            player1Id,
            player2Id
        );

        if (playerId == match.Player1Id)
            match.Player1Connected = true;

        if (playerId == match.Player2Id)
            match.Player2Connected = true;

        if (match.Player1Connected &&
            match.Player2Connected)
        {
            match.Phase = MatchPhases.ReadyForCoinFlip;

            await Clients.Group(setId.ToString())
                .SendAsync(
                    MatchPhases.BothPlayersConnected
                );
        }
        else
        {
            await Clients.Group(setId.ToString())
                .SendAsync(
                    MatchPhases.WaitingForPlayers
                );
        }

        await _matches.SaveAsync();

        var state =
            await _matches.GetMatchStateAsync(match.Id);

        await Clients.Caller.SendAsync(
            "MatchState",
            state
        );
    }

    public async Task FlipCoin(long setId)
    {
        var match =
            await _matches.GetBySetIdAsync(setId);

        if (match == null)
            return;

        if (match.Phase != MatchPhases.ReadyForCoinFlip)
            return;

        if (match.CoinWinnerPlayerId != null)
            return;

        var winner =
            Random.Shared.Next(2) == 0
            ? match.Player1Id
            : match.Player2Id;

        match.CoinWinnerPlayerId = winner;
        match.CurrentTurnPlayerId = winner;
        match.Phase = MatchPhases.CoinFlipped;

        await _matches.SaveAsync();

        await Clients.Group(setId.ToString())
            .SendAsync(
                MatchPhases.CoinResult,
                winner,
                winner
            );
    }

    public async Task SetStageState(
        long setId,
        int stageId,
        string state,
        long playerId)
    {
        var match =
            await _matches.GetBySetIdAsync(setId);

        if (match == null)
            return;

        await _matches.SetStageStateAsync(
            match.Id,
            stageId,
            state,
            playerId
        );

        await Clients.Group(setId.ToString())
            .SendAsync(
                "StageStateUpdated",
                stageId,
                state,
                playerId
            );
    }

    public async Task ResetStages(long setId)
    {
        var match =
            await _matches.GetBySetIdAsync(setId);

        if (match == null)
            return;

        await _matches.ResetStagesAsync(match.Id);

        await Clients.Group(setId.ToString())
            .SendAsync(
                "StagesReset"
            );
    }

    public async Task SubmitResult(
        long setId,
        long reporterId,
        long winnerPlayerId,
        string winnerTag,
        long loserPlayerId,
        string loserTag,
        int winnerScore,
        int loserScore)
    {
        var match =
            await _matches.GetBySetIdAsync(setId);

        if (match == null)
            return;

        if (reporterId != match.Player1Id &&
            reporterId != match.Player2Id)
        {
            return;
        }

        var report = new MatchResultReport
        {
            SetId = setId,
            MatchSessionId = match.Id,
            ReporterPlayerId = reporterId,

            WinnerPlayerId = winnerPlayerId,
            WinnerTag = winnerTag,

            LoserPlayerId = loserPlayerId,
            LoserTag = loserTag,

            WinnerScore = winnerScore,
            LoserScore = loserScore
        };

        var reports =
            _pendingReports.GetOrAdd(
                setId,
                _ => new List<MatchResultReport>()
            );

        lock (reports)
        {
            reports.RemoveAll(
                x => x.ReporterPlayerId == reporterId
            );

            reports.Add(report);
        }

        if (reports.Count < 2)
        {
            await Clients.Caller.SendAsync(
                "WaitingOpponentReport"
            );

            return;
        }

        MatchResultReport first;
        MatchResultReport second;

        lock (reports)
        {
            first = reports[0];
            second = reports[1];
        }

        bool sameResult =
            first.WinnerPlayerId == second.WinnerPlayerId &&
            first.LoserPlayerId == second.LoserPlayerId &&
            first.WinnerScore == second.WinnerScore &&
            first.LoserScore == second.LoserScore;

        var tournament =
            await _startGg.GetTournamentInfoAsync(
                match.TournamentId
            );
        var eventName =
            await _startGg.GetEventNameAsync(
                match.EventId
            );
        if (!sameResult)
        {
            await Clients.Group(setId.ToString())
                .SendAsync(
                    "ResultConflict",
                    "Los reportes no coinciden."
                );

            var canNotify =
                !_lastConflictNotification.ContainsKey(setId) ||
                DateTime.UtcNow -
                _lastConflictNotification[setId]
                > TimeSpan.FromSeconds(30);

            if (canNotify &&
                tournament != null &&
                !string.IsNullOrWhiteSpace(tournament.Email))
            {
                var message =
$"""
Alerta de conflicto de resultado

Torneo:
{tournament.Name}

Evento:
{eventName}

Primer reporte:

{first.WinnerTag}   {first.WinnerScore} - {first.LoserScore}   {first.LoserTag}

Segundo reporte:

{second.WinnerTag}   {second.WinnerScore} - {second.LoserScore}   {second.LoserTag}

Los jugadores requieren revisión manual.

Mensaje automático de BracketSmasher.
""";

                await _email.SendEmailAsync(
                    tournament.Email,
                    "BracketSmasher - Conflicto de resultado",
                    message
                );

                _lastConflictNotification[setId] =
                    DateTime.UtcNow;
            }

            _pendingReports.TryRemove(
                setId,
                out _
            );

            return;
        }

        match.Phase = "ResultConfirmed";

        await _matches.SaveAsync();

        await Clients.Group(setId.ToString())
            .SendAsync(
                "ResultConfirmed"
            );

        if (tournament != null &&
            !string.IsNullOrWhiteSpace(tournament.Email))
        {
            var stages =
                await _matches.GetSelectedStagesAsync(
                    match.Id
                );

            var setup =
                string.Join(
                    ", ",
                    stages
                );

            var message =
$"""
Nuevo resultado confirmado

Torneo:
{tournament.Name}

Evento:
{eventName}

Resultado:

{first.WinnerTag}   {first.WinnerScore} - {first.LoserScore}   {first.LoserTag}

Setup:
{setup}

Resultado confirmado por ambos jugadores.

Mensaje automático de BracketSmasher.
""";

            await _email.SendEmailAsync(
                tournament.Email,
                "BracketSmasher - Resultado confirmado",
                message
            );
        }

        _pendingReports.TryRemove(
            setId,
            out _
        );
    }
}