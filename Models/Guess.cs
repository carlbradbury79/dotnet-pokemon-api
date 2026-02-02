namespace pokemonApi.Models;

public class Guess
{
    public int Id { get; set; }
    
    public int GameId { get; set; }
    
    public string GuessedWord { get; set; } = null!;
    
    /// <summary>
    /// JSON array storing letter status results.
    /// Example: [{"letter":"P","status":"Correct"},{"letter":"I","status":"WrongPosition"}]
    /// This will be used to display green/yellow/gray letters to the user.
    /// </summary>
    public string LetterResults { get; set; } = null!;
    
    public int AttemptNumber { get; set; }
    
    public DateTime GuessedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Game Game { get; set; } = null!;
}

public enum LetterStatus
{
    Correct,        // Green - correct position
    WrongPosition,  // Yellow - in word, wrong position
    NotInWord       // Gray - not in word
}
