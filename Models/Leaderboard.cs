namespace pokemonApi.Models;

public class Leaderboard
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int DailyPokemonId { get; set; }
    
    public int TimeToSolveSeconds { get; set; }
    
    public int Attempts { get; set; }
    
    public DateTime SolvedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public DailyPokemon DailyPokemon { get; set; } = null!;
}
