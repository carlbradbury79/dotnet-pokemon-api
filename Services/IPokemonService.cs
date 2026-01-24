using PokemonApi.Models;

namespace PokemonApi.Services;

public interface IPokemonService
{
    Task<PokemonApiResponse?> GetPokemonAsync(string name);
}
