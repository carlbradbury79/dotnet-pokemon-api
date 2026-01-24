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
}
