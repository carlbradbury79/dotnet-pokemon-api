// Learning Point: This entity stores each guess attempt in a game.
// It will help us track user progress and calculate statistics later.
namespace pokemonApi.Models;

public class Guess
{
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to Game.
    /// </summary>
    public int GameId { get; set; }
    
    /// <summary>
    /// The user's guess (e.g., "PIKACHU").
    /// </summary>
    public string GuessedWord { get; set; } = null!;
    
    /// <summary>
    /// JSON array storing letter status results.
    /// Example: [{"letter":"P","status":"Correct"},{"letter":"I","status":"WrongPosition"}]
    /// This will be used to display green/yellow/gray letters to the user.
    /// </summary>
    public string LetterResults { get; set; } = null!;
    
    /// <summary>
    /// Sequence number of this guess (1st attempt, 2nd attempt, etc).
    /// </summary>
    public int AttemptNumber { get; set; }
    
    public DateTime GuessedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Game Game { get; set; } = null!;
}

/// <summary>
/// Represents the status of a single letter in the guess.
/// Learning Point: This enum maps to the Wordle game logic (green=correct, yellow=wrong position, gray=not in word).
/// </summary>
public enum LetterStatus
{
    Correct,        // Green - letter is in the correct position
    WrongPosition,  // Yellow - letter is in the word but wrong position
    NotInWord       // Gray - letter is not in the word
}
