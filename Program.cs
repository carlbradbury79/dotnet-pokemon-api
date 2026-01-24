using PokemonApi.Services;
using PokemonApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Bind settings from appsettings.json
builder.Services.Configure<PokemonApiSettings>(
    builder.Configuration.GetSection("PokemonApi"));

// Register HttpClient + your service
builder.Services.AddHttpClient<IPokemonService, PokemonService>();

var app = builder.Build();

app.MapGet("/", () => "Pokémon API is running!");

app.MapGet("/pokemon/{name}", async (string name, IPokemonService pokemonService) =>
{
    var pokemon = await pokemonService.GetPokemonAsync(name);
    
    if (pokemon == null)
    {
        return Results.NotFound(new { message = $"Pokemon '{name}' not found" });
    }
    
    return Results.Ok(pokemon);
});

app.Run();
