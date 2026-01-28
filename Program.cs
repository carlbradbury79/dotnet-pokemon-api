using PokemonApi.Services;
using PokemonApi.Settings;
using Microsoft.EntityFrameworkCore;
using pokemonApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Learning Point: Add DbContext with PostgreSQL connection string
// The connection string is read from appsettings.json (or env vars in production)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PokemonDbContext>(options =>
    options.UseNpgsql(connectionString)
);

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

app.MapGet("/random", async (IPokemonService pokemonService) =>
{
    var pokemon = await pokemonService.GetRandomPokemonAsync();
    if (pokemon == null)
    {
        return Results.NotFound(new { message = "No random Pokemon found" });
    }
    return Results.Ok(pokemon);
});

app.Run();
