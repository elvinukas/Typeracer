using Microsoft.EntityFrameworkCore;
using Typeracer.Models;

namespace Typeracer.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Paragraph> Paragraphs { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<WPM> Wpms { get; set; }
    public DbSet<Accuracy> Accuracies { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<StatisticsModel> Statistics { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WPM>()
            .HasOne<Player>()
            .WithMany(p => p.WPMs)
            .HasForeignKey(w => w.PlayerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Accuracy>()
            .HasOne<Player>()
            .WithMany(p => p.Accuracies)
            .HasForeignKey(a => a.PlayerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Player>()
            .HasKey(p => p.PlayerID);

        modelBuilder.Entity<Player>()
            .HasOne(p => p.BestWPM)
            .WithOne()
            .HasForeignKey<Player>(p => p.BestWPMID)
            .IsRequired(false);

        modelBuilder.Entity<Player>()
            .HasOne(p => p.BestAccuracy)
            .WithOne()
            .HasForeignKey<Player>(p => p.BestAccuracyID)
            .IsRequired(false);

        modelBuilder.Entity<Game>()
            .HasKey(g => g.GameId);
        
        modelBuilder.Entity<Game>()
            .HasOne(g => g.Player)
            .WithMany()
            .HasForeignKey(g => g.PlayerId)
            .IsRequired(false);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Statistics)
            .WithOne()
            .HasForeignKey<Game>(g => g.StatisticsId);

        modelBuilder.Entity<StatisticsModel>()
            .HasKey(s => s.StatisticsId);

        modelBuilder.Entity<StatisticsModel>()
            .HasOne(s => s.Paragraph)
            .WithMany()
            .HasForeignKey(s => s.ParagraphId);
        
        modelBuilder.Entity<Paragraph>()
            .HasKey(p => p.Id);

       
    }
    
   
}