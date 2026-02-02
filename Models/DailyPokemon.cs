namespace pokemonApi.Models;

public class DailyPokemon
{
    public int Id { get; set; }
    
    public DateOnly Date { get; set; }
    
    public int PokemonId { get; set; }
    
    public string PokemonName { get; set; } = null!;
    
    public string ImageUrl { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
