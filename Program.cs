using PokemonApi.Services;
using PokemonApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// PHASE 1: Basic API Setup
// Learning: Minimal APIs, Dependency Injection, Configuration
// ============================================

// 1. Configure PokemonApiSettings from appsettings.json
//    This reads the "PokemonApi" section and makes it available throughout the app
builder.Services.Configure<PokemonApiSettings>(
    builder.Configuration.GetSection("PokemonApi"));

// 2. Register HttpClient and PokemonService
//    Dependency Injection: We register the service so it's automatically provided where needed
//    The using statement imports the namespace containing these interfaces
builder.Services.AddHttpClient<IPokemonService, PokemonService>();

var app = builder.Build();

// ============================================
// ENDPOINTS
// ============================================

// Health check - verify API is running
app.MapGet("/", () => "Pokémon API is running!")
    .WithName("HealthCheck")
    .WithDescription("Verify the API is running");

// Get a specific Pokemon by name
// Example: GET /pokemon/pikachu
// Explanation: 
//   - {name} is a route parameter
//   - IPokemonService is injected by the DI container
//   - async/await means the thread isn't blocked while waiting for external API
app.MapGet("/pokemon/{name}", async (string name, IPokemonService pokemonService) =>
{
    var pokemon = await pokemonService.GetPokemonAsync(name);
    
    if (pokemon == null)
    {
        return Results.NotFound(new { message = $"Pokemon '{name}' not found" });
    }
    
    return Results.Ok(pokemon);
})
.WithName("GetPokemon")
.WithDescription("Get a specific Pokemon by name");

// Get a random Pokemon
// This demonstrates more complex service logic
app.MapGet("/random", async (IPokemonService pokemonService) =>
{
    var pokemon = await pokemonService.GetRandomPokemonAsync();
    
    if (pokemon == null)
    {
        return Results.NotFound(new { message = "No random Pokemon found" });
    }
    
    return Results.Ok(pokemon);
})
.WithName("GetRandomPokemon")
.WithDescription("Get a random Pokemon");

app.Run();
