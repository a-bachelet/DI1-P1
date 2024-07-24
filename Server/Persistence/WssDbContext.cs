using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Persistence;

public class WssDbContext(DbContextOptions options, IConfiguration configuration) : DbContext(options)
{
  public DbSet<Game> Games { get; set; } = null!;
  public DbSet<Player> Players { get; set; } = null!;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    var dbOptions = configuration.GetSection("Database");
    
    var dbHost = dbOptions.GetValue<string>("Host");
    var dbPort = dbOptions.GetValue<string>("Port");
    var dbName = dbOptions.GetValue<string>("Name");
    var dbUser = dbOptions.GetValue<string>("User");
    var dbPass = dbOptions.GetValue<string>("Pass");
    
    var dbConnectionString = $"Host={dbHost};Port={dbPort};Db={dbName};Username={dbUser};Password={dbPass}";
    
    optionsBuilder.UseNpgsql(dbConnectionString);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Game>(e => {
      e.ToTable("games");
      e.HasKey(e => e.Id);
      e.Property(e => e.Name).HasColumnType("VARCHAR(255)");
      e.Property(e => e.Rounds).HasColumnType("INTEGER");
      e.HasMany(e => e.Players).WithOne(e => e.Game).HasForeignKey(e => e.GameId);
    });

    modelBuilder.Entity<Player>(e => {
      e.ToTable("players");
      e.HasKey(e => e.Id);
      e.Property(e => e.Name).HasColumnType("VARCHAR(255)");
      e.HasOne(e => e.Game).WithMany(e => e.Players).HasForeignKey(e => e.GameId);
    });
  }
}
