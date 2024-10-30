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
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<WPM>()
            .HasOne(w => w.Player)
            .WithMany(p => p.WPMs)
            .HasForeignKey(w => w.PlayerId)
            .OnDelete(DeleteBehavior.Cascade); 

      
        modelBuilder.Entity<Accuracy>()
            .HasOne(a => a.Player)
            .WithMany(p => p.Accuracies)
            .HasForeignKey(a => a.PlayerId)
            .OnDelete(DeleteBehavior.Cascade); 

        
        modelBuilder.Entity<Paragraph>()
            .HasKey(p => p.Id); 

        
        modelBuilder.Entity<Player>()
            .HasKey(p => p.PlayerID); 

       
    }
    
   
}