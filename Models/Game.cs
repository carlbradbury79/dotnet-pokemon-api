// Learning Point: This entity represents a single game session.
// It tracks the Pokemon being guessed, the player's attempts, and the outcome.
namespace pokemonApi.Models;

public class Game
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to User - required relationship.
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// The Pokemon ID for this day's puzzle (from PokéAPI).
    /// All users see the same daily Pokemon.
    /// </summary>
    public int DailyPokemonId { get; set; }
    
    /// <summary>
    /// The Pokemon name (cached for display).
    /// </summary>
    public string PokemonName { get; set; } = null!;
    
    /// <summary>
    /// Game status: Active, Won, Lost
    /// </summary>
    public GameStatus Status { get; set; } = GameStatus.Active;
    
    /// <summary>
    /// Number of attempts made so far.
    /// </summary>
    public int AttemptCount { get; set; } = 0;
    
    /// <summary>
    /// Whether the user won this game.
    /// </summary>
    public bool IsWon { get; set; } = false;
    
    /// <summary>
    /// Time elapsed in seconds when won (null if not yet won).
    /// </summary>
    public int? TimeToSolveSeconds { get; set; }
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Guess> Guesses { get; set; } = new List<Guess>();
}

/// <summary>
/// Represents the state of a game.
/// </summary>
public enum GameStatus
{
    Active,
    Won,
    Lost
}
