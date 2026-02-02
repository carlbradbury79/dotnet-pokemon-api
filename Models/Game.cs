namespace pokemonApi.Models;

public class Game
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int DailyPokemonId { get; set; }
    
    public string PokemonName { get; set; } = null!;
    
    public GameStatus Status { get; set; } = GameStatus.Active;
    
    public int AttemptCount { get; set; } = 0;
    
    public bool IsWon { get; set; } = false;
    
    public int? TimeToSolveSeconds { get; set; }
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Guess> Guesses { get; set; } = new List<Guess>();
}

public enum GameStatus
{
    Active,
    Won,
    Lost
}
