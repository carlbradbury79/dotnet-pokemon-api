// Learning Point: This entity stores leaderboard entries per Pokemon.
// It records the fastest time for each user to complete each day's puzzle.
namespace pokemonApi.Models;

public class Leaderboard
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to User.
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Foreign key to DailyPokemon.
    /// </summary>
    public int DailyPokemonId { get; set; }
    
    /// <summary>
    /// Time in seconds to solve this puzzle.
    /// </summary>
    public int TimeToSolveSeconds { get; set; }
    
    /// <summary>
    /// Number of attempts taken to solve.
    /// </summary>
    public int Attempts { get; set; }
    
    /// <summary>
    /// Timestamp when this puzzle was solved.
    /// </summary>
    public DateTime SolvedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public DailyPokemon DailyPokemon { get; set; } = null!;
}
