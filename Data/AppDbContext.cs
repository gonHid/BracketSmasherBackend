using BracketSmasherBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BracketSmasherBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchSession>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<MatchResultReport>()
        .HasKey(x => x.Id);
        // StageBan PK
        modelBuilder.Entity<StageBan>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<StageBan>()
            .HasOne(x => x.MatchSession)
            .WithMany(x => x.StageBans)
            .HasForeignKey(x => x.MatchSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchResultReport>()
            .HasOne(x => x.MatchSession)
            .WithMany(x => x.ResultReports)
            .HasForeignKey(x => x.MatchSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    public DbSet<MatchSession> MatchSessions => Set<MatchSession>();
    public DbSet<StageBan> StageBans => Set<StageBan>();
    public DbSet<StageState> StageStates => Set<StageState>();
    public DbSet<MatchResultReport> MatchResultReports { get; set; }
}