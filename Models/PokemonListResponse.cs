namespace PokemonApi.Models;

public class PokemonListResponse
{
    public List<RandomPokemonApiResponse> Results { get; set; } = new();
}

public class RandomPokemonApiResponse
{
    public string Name { get; set; } = string.Empty;
 
    public string Url { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}
