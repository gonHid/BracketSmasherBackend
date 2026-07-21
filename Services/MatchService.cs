using BracketSmasherBackend.Data;
using BracketSmasherBackend.DTOs;
using BracketSmasherBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BracketSmasherBackend.Services;

public class MatchService
{
    private readonly AppDbContext _db;


    public MatchService(AppDbContext db)
    {
        _db = db;
    }



    public async Task<MatchSession> GetOrCreateAsync(
        long tournamentId,
        long eventId,
        long setId,
        DateTime tournamentStartAt,
        long player1Id,
        long player2Id)
    {
        var match =
            await _db.MatchSessions
            .FirstOrDefaultAsync(
                x => x.SetId == setId
            );


        if (match != null)
            return match;



        match = new MatchSession
        {
            TournamentId = tournamentId,
            EventId = eventId,
            SetId = setId,

            CreatedAt = tournamentStartAt,

            ExpiresAt =
                tournamentStartAt.AddDays(7),

            Player1Id = player1Id,
            Player2Id = player2Id,

            Phase = "WaitingPlayers"
        };


        _db.MatchSessions.Add(match);

        await _db.SaveChangesAsync();


        return match;
    }



    public async Task<MatchSession?> GetBySetIdAsync(long setId)
    {
        return await _db.MatchSessions
            .FirstOrDefaultAsync(
                x => x.SetId == setId
            );
    }



    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }



    public async Task SaveResultReportAsync(
        MatchResultReport report)
    {
        _db.MatchResultReports.Add(report);

        await _db.SaveChangesAsync();
    }



    public async Task<List<MatchResultReport>>
        GetResultReportsAsync(long setId)
    {
        return await _db.MatchResultReports
            .Where(x => x.SetId == setId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }



    public async Task RemoveResultReportsAsync(long setId)
    {
        var reports =
            await _db.MatchResultReports
            .Where(x => x.SetId == setId)
            .ToListAsync();


        _db.MatchResultReports.RemoveRange(reports);


        await _db.SaveChangesAsync();
    }



    public async Task SetStageStateAsync(
        Guid matchSessionId,
        int stageId,
        string state,
        long playerId)
    {

        if (state == "selected")
        {
            var selected =
                await _db.StageStates
                .Where(x =>
                    x.MatchSessionId == matchSessionId &&
                    x.State == "selected")
                .ToListAsync();


            foreach (var s in selected)
                s.State = "neutral";
        }



        var stage =
            await _db.StageStates
            .FirstOrDefaultAsync(x =>
                x.MatchSessionId == matchSessionId &&
                x.StageId == stageId);



        if (stage == null)
        {
            stage = new StageState
            {
                MatchSessionId = matchSessionId,
                StageId = stageId
            };

            _db.StageStates.Add(stage);
        }



        stage.State = state;
        stage.UpdatedByPlayerId = playerId;
        stage.UpdatedAt = DateTime.UtcNow;



        await _db.SaveChangesAsync();
    }



    public async Task ResetStagesAsync(Guid matchSessionId)
    {
        var stages =
            await _db.StageStates
            .Where(x =>
                x.MatchSessionId == matchSessionId)
            .ToListAsync();



        foreach (var s in stages)
        {
            s.State = "neutral";
            s.UpdatedAt = DateTime.UtcNow;
        }



        await _db.SaveChangesAsync();
    }



    public async Task<MatchStateDto>
        GetMatchStateAsync(Guid matchSessionId)
    {
        var match =
            await _db.MatchSessions
            .FirstAsync(
                x => x.Id == matchSessionId
            );



        var stages =
            await _db.StageStates
            .Where(x =>
                x.MatchSessionId == matchSessionId)
            .Select(x => new StageStateDto
            {
                StageId = x.StageId,
                State = x.State
            })
            .ToListAsync();



        return new MatchStateDto
        {
            Phase = match.Phase,

            CoinWinnerPlayerId =
                match.CoinWinnerPlayerId,

            CurrentTurnPlayerId =
                match.CurrentTurnPlayerId,

            Stages = stages
        };
    }
    public async Task<List<int>> GetSelectedStagesAsync(
    Guid matchSessionId)
    {
        return await _db.StageStates
            .Where(x =>
                x.MatchSessionId == matchSessionId &&
                x.State == "selected")
            .Select(x => x.StageId)
            .ToListAsync();
    }
}