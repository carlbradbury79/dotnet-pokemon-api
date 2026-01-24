using Microsoft.Extensions.Options;
using PokemonApi.Models;
using PokemonApi.Settings;

namespace PokemonApi.Services;

public class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;
    private readonly PokemonApiSettings _settings;

    public PokemonService(HttpClient httpClient, IOptions<PokemonApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<PokemonApiResponse?> GetPokemonAsync(string name)
    {
        var url = $"{_settings.BaseUrl}pokemon/{name.ToLower()}";

        try
        {
            return await _httpClient.GetFromJsonAsync<PokemonApiResponse>(url);
        }
        catch (HttpRequestException)
        {
            // Network or API error
            return null;
        }
        catch (NotSupportedException)
        {
            // JSON parse error
            return null;
        }
    }

    public async Task<RandomPokemonApiResponse?> GetRandomPokemonAsync()
    {
        var randomPokemonNumber = Random.Shared.Next(1, _settings.MaxPokemonCount + 1); 
        var url = $"{_settings.BaseUrl}pokemon?limit=1&offset={randomPokemonNumber - 1}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<PokemonListResponse>(url);
            if (response == null || response.Results == null || response.Results.Count == 0)
            {
                return null;
            }

            var pokemonFromList = response.Results[0];
            
            // Fetch detailed pokemon data to get sprites
            var detailedPokemon = await _httpClient.GetFromJsonAsync<PokemonApiResponse>(pokemonFromList.Url);
            if (detailedPokemon?.Sprites != null && !string.IsNullOrEmpty(detailedPokemon.Sprites.FrontDefault))
            {
                pokemonFromList.ImageUrl = detailedPokemon.Sprites.FrontDefault;
            }
            
            return pokemonFromList;
        }
        catch (HttpRequestException)
        {
            // Network or API error
            return null;
        }
        catch (NotSupportedException)
        {
            // JSON parse error
            return null;
        }
    }
}
