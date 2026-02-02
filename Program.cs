using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pokemonApi.Data;
using pokemonApi.Services;
using pokemonApi.Settings;
using PokemonApi.Services;
using PokemonApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Configure JWT settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// Add authentication services
var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Learning Point: Add DbContext with PostgreSQL connection string
// The connection string is read from appsettings.json (or env vars in production)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PokemonDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Bind settings from appsettings.json
builder.Services.Configure<PokemonApiSettings>(
    builder.Configuration.GetSection("PokemonApi"));

// Register services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IPokemonService, PokemonService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Pokémon API is running!");

// Auth endpoints
app.MapPost("/auth/register", async (RegisterRequest request, IAuthService authService) =>
{
    var response = await authService.RegisterAsync(request);
    return response.Success ? Results.Ok(response) : Results.BadRequest(response);
});

app.MapPost("/auth/login", async (LoginRequest request, IAuthService authService) =>
{
    var response = await authService.LoginAsync(request);
    return response.Success ? Results.Ok(response) : Results.Unauthorized();
});

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
