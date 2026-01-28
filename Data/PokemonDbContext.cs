// Learning Point: DbContext is the bridge between our .NET models and the database.
// It handles all database operations through EF Core. Configuration happens in OnModelCreating.
using Microsoft.EntityFrameworkCore;
using pokemonApi.Models;

namespace pokemonApi.Data;

public class PokemonDbContext : DbContext
{
    public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options)
    {
    }

    // DbSets represent tables in the database
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Guess> Guesses { get; set; }
    public DbSet<DailyPokemon> DailyPokemon { get; set; }
    public DbSet<Leaderboard> Leaderboards { get; set; }

    /// <summary>
    /// Learning Point: OnModelCreating is where we configure entity relationships and constraints.
    /// This runs when the DbContext is initialized and helps EF Core understand our schema.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            // Learning Point: Unique constraint ensures no duplicate usernames
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Configure Game entity
        modelBuilder.Entity<Game>(entity =>
        {
            // Learning Point: Foreign key with cascade delete - if user is deleted, their games are too
            entity.HasOne(e => e.User)
                .WithMany(u => u.Games)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.PokemonName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
        });

        // Configure Guess entity
        modelBuilder.Entity<Guess>(entity =>
        {
            entity.HasOne(e => e.Game)
                .WithMany(g => g.Guesses)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.GuessedWord).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LetterResults).IsRequired();
        });

        // Configure DailyPokemon entity
        modelBuilder.Entity<DailyPokemon>(entity =>
        {
            // Learning Point: Unique constraint on Date - only one Pokemon per day
            entity.HasIndex(e => e.Date).IsUnique();
            entity.Property(e => e.PokemonName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ImageUrl).IsRequired();
        });

        // Configure Leaderboard entity
        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DailyPokemon)
                .WithMany()
                .HasForeignKey(e => e.DailyPokemonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Learning Point: Composite unique index - one leaderboard entry per user per daily pokemon
            entity.HasIndex(e => new { e.UserId, e.DailyPokemonId }).IsUnique();
        });
    }
}
