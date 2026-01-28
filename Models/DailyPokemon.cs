// Learning Point: This entity represents the daily Pokemon challenge.
// We'll use a UTC-based seed to ensure all users see the same Pokemon each day.
namespace pokemonApi.Models;

public class DailyPokemon
{
    public int Id { get; set; }
    
    /// <summary>
    /// The date this Pokemon is active (UTC date only, no time).
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// Pokemon ID from PokéAPI (1-1025).
    /// </summary>
    public int PokemonId { get; set; }
    
    /// <summary>
    /// Pokemon name for caching.
    /// </summary>
    public string PokemonName { get; set; } = null!;
    
    /// <summary>
    /// URL to the Pokemon's image.
    /// </summary>
    public string ImageUrl { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
