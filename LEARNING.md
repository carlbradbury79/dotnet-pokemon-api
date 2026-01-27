# Pokemon Wordle API - Learning Guide

This guide explains the codebase step-by-step, organized by learning phase.

## Phase 1: Basic API (Foundation)

### What You're Learning
- **Minimal APIs** — Modern .NET way to build APIs without controllers
- **Dependency Injection** — How services are registered and used
- **HttpClient** — Making external HTTP requests
- **Configuration** — Reading settings from `appsettings.json`

### Key Files
- **Program.cs** — API setup, endpoint definitions
- **Services/IPokemonService.cs** — Business logic for fetching Pokemon
- **Settings/PokemonApiSettings.cs** — Configuration model
- **appsettings.json** — Configuration values

### Concepts Explained

#### 1. Program.cs Structure
```csharp
var builder = WebApplication.CreateBuilder(args);  // Setup
builder.Services.AddHttpClient<IPokemonService, PokemonService>();  // Register service
var app = builder.Build();  // Build
app.MapGet("/", () => "...");  // Define endpoints
app.Run();  // Start
```

**What's happening:**
- `builder` = configuration container
- `builder.Services` = dependency injection container
- `app.MapGet()` = define a GET endpoint
- Services are **injected** into endpoint handlers

#### 2. Dependency Injection
```csharp
app.MapGet("/pokemon/{name}", async (string name, IPokemonService pokemonService) =>
{
    var pokemon = await pokemonService.GetPokemonAsync(name);
    return Results.Ok(pokemon);
});
```

**Why this matters:**
- `IPokemonService pokemonService` is **automatically provided** by .NET
- You don't manually create it
- Makes testing easy (can swap implementations)
- Follows **Dependency Inversion Principle**

#### 3. Services Pattern
```csharp
public interface IPokemonService
{
    Task<PokemonApiResponse?> GetPokemonAsync(string name);
    Task<RandomPokemonApiResponse?> GetRandomPokemonAsync();
}

public class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;
    private readonly PokemonApiSettings _settings;
    
    // Dependencies injected via constructor
    public PokemonService(HttpClient httpClient, IOptions<PokemonApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }
}
```

**Key points:**
- Interface `IPokemonService` defines the contract
- Class `PokemonService` implements it
- Dependencies (HttpClient, Settings) are constructor parameters
- All injected by .NET automatically

---

## Phase 2A: Database Setup (EF Core Basics)

### What You're Learning
- **Entity Framework Core (EF Core)** — ORM for database access
- **DbContext** — Represents database connection and tables
- **Migrations** — Version control for database schema
- **PostgreSQL** — Relational database

### Key Files
- **Data/GameDbContext.cs** — EF Core configuration, table definitions
- **appsettings.json** — Database connection string
- **Migrations/** — Database schema versions

### Concepts Explained

#### 1. DbContext
```csharp
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }
    
    // Define tables
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PokemonStatistic> PokemonStatistics => Set<PokemonStatistic>();
    public DbSet<GameRound> GameRounds => Set<GameRound>();
}
```

**What this does:**
- Inherits from `DbContext` (EF Core base class)
- `DbSet<T>` represents a table
- `Players` = the `Players` table in database
- Each property maps to a table

#### 2. Program.cs Database Registration
```csharp
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**What this does:**
- Registers `GameDbContext` in dependency injection
- Uses PostgreSQL (`UseNpgsql`)
- Gets connection string from `appsettings.json`

#### 3. Connection String
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=pokemon_wordle;Username=postgres;Password=postgres"
}
```

**Parts:**
- `Host` = where database runs
- `Port` = database port (5432 is PostgreSQL default)
- `Database` = database name
- `Username/Password` = authentication

#### 4. Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**What happens:**
- `migrations add` = compares model to database, creates migration file
- Migration file = SQL instructions to create tables
- `database update` = runs migrations, applies schema to database
- Creates `Migrations/` folder with version history

---

## Phase 2B: Player Entities & Data Modeling

### What You're Learning
- **Entity Relationships** — Foreign keys, one-to-many relationships
- **Data Annotations** — Configuring how entities map to tables
- **LINQ** — Querying databases with C# syntax

### Key Files
- **Models/Player.cs** — User entity
- **Models/PokemonStatistic.cs** — Game performance per Pokemon
- **Models/GameRound.cs** — Individual game attempt record

### Concepts Explained

#### 1. Entity Structure
```csharp
public class Player
{
    public Guid Id { get; set; }  // Primary key
    public string Username { get; set; }  // Username
    public string PasswordHash { get; set; }  // Hashed password (never store plaintext!)
    
    // Navigation properties - relationships to other tables
    public ICollection<PokemonStatistic> PokemonStatistics { get; set; }
    public ICollection<GameRound> GameRounds { get; set; }
}
```

**Key concepts:**
- `Guid Id` = primary key (unique identifier)
- `ICollection<>` = one-to-many relationship
- One player has many statistics and game rounds

#### 2. Relationships
```csharp
public class PokemonStatistic
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }  // Foreign key
    
    public Player Player { get; set; }  // Navigation property
}
```

**Database result:**
```
Players table:
  Id: 123
  Username: player1

PokemonStatistics table:
  Id: 456
  PlayerId: 123 (references Players.Id)
```

#### 3. Fluent API Configuration
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Player>(entity =>
    {
        entity.HasKey(e => e.Id);  // Primary key
        entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
        entity.HasMany(e => e.PokemonStatistics)
            .WithOne(p => p.Player)
            .HasForeignKey(p => p.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);  // Delete stats if player deleted
    });
}
```

**What this configures:**
- Primary keys
- Required fields
- Field constraints (max length)
- Relationships
- Delete behavior (cascade = delete related records)

---

## Phase 3: Player Statistics & Business Logic

### What You're Learning
- **Service Layer** — Business logic separate from API
- **CRUD Operations** — Create, Read, Update, Delete
- **Aggregation** — Calculating statistics from raw data
- **Async/Await** — Non-blocking database operations

### Key Files
- **Services/IPlayerService.cs** — Player logic and statistics

### Concepts Explained

#### 1. Service Interface
```csharp
public interface IPlayerService
{
    Task<Player> GetOrCreatePlayerAsync(string username);
    Task<Player?> GetPlayerAsync(Guid playerId);
    Task RecordGameResultAsync(Guid playerId, int pokemonId, string pokemonName, 
        int attemptsUsed, bool isWon, double? timeTakenSeconds);
    Task<List<PokemonStatistic>> GetPlayerStatisticsAsync(Guid playerId);
}
```

**Why an interface:**
- Defines what the service can do
- Easy to test (mock the interface)
- Can swap implementations later

#### 2. Async/Await Pattern
```csharp
public async Task<Player?> GetPlayerAsync(Guid playerId)
{
    return await _context.Players
        .FirstOrDefaultAsync(p => p.Id == playerId);
}
```

**Why async:**
- Database queries are I/O bound (slow)
- `await` releases thread while waiting
- Thread can handle other requests
- Performance: one thread handles 100 requests vs 100 threads

#### 3. Recording Game Results
```csharp
public async Task RecordGameResultAsync(Guid playerId, int pokemonId, 
    string pokemonName, int attemptsUsed, bool isWon, double? timeTakenSeconds)
{
    // Create game round record
    var gameRound = new GameRound { ... };
    _context.GameRounds.Add(gameRound);
    
    // Update or create statistics
    var statistic = await _context.PokemonStatistics
        .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.PokemonId == pokemonId);
    
    if (isWon)
    {
        statistic.TotalWins++;
        statistic.AverageAttemptsToWin = 
            (statistic.AverageAttemptsToWin * (statistic.TotalWins - 1) + attemptsUsed) 
            / statistic.TotalWins;
    }
    
    await _context.SaveChangesAsync();
}
```

**Key pattern:**
1. Find existing record
2. Update fields
3. Calculate aggregates
4. `SaveChangesAsync()` = send to database

#### 4. Statistics Calculation
```csharp
var summary = new
{
    totalWins = statistics.Sum(s => s.TotalWins),
    totalLosses = statistics.Sum(s => s.TotalLosses),
    overallWinRate = (double)statistics.Sum(s => s.TotalWins) 
        / statistics.Sum(s => s.TotalWins + s.TotalLosses),
    averageAttemptsToWin = statistics.Where(s => s.TotalWins > 0)
        .Average(s => s.AverageAttemptsToWin)
};
```

**LINQ methods:**
- `Sum()` — Add up values
- `Where()` — Filter
- `Average()` — Calculate mean

---

## Phase 4A: Authentication Service

### What You're Learning
- **Hashing & Security** — Password storage with bcrypt
- **JWT Tokens** — Stateless authentication
- **Claims-Based Identity** — Who is the user?

### Key Files
- **Services/IAuthService.cs** — Registration and login
- **Settings/JwtSettings.cs** — JWT configuration

### Concepts Explained

#### 1. Password Hashing
```csharp
public async Task<AuthResponse?> RegisterAsync(string username, string password)
{
    // Hash password with bcrypt
    var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
    
    var player = new Player
    {
        Username = username,
        PasswordHash = passwordHash  // Store hash, NOT plaintext
    };
}
```

**Why bcrypt:**
- One-way function (can't reverse)
- Slow (resists brute-force attacks)
- Includes salt (random data) - same password hashes differently
- Never store passwords in plaintext!

#### 2. Verification
```csharp
if (!BCrypt.Net.BCrypt.Verify(password, player.PasswordHash))
{
    return null;  // Wrong password
}
```

**What `Verify` does:**
1. Hash input password with salt from stored hash
2. Compare hashes
3. Return true if match

#### 3. JWT Token Generation
```csharp
private string GenerateJwtToken(Player player)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
        new Claim(ClaimTypes.Name, player.Username)
    };
    
    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        signingCredentials: credentials
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**Parts:**
- `claims` = user information embedded in token
- `SigningCredentials` = secret key + algorithm
- `JwtSecurityToken` = create the token
- Token is signed so it can't be tampered with

#### 4. JWT Structure
```
Header.Payload.Signature

Header: { "alg": "HS256", "typ": "JWT" }
Payload: { "nameid": "player-id", "name": "username", "exp": 1234567890 }
Signature: HMAC-SHA256(header.payload, secret)
```

**Why JWT:**
- Stateless (no server session needed)
- Client carries all info
- Verifiable (signature proves it's real)
- Can include metadata (username, roles, etc.)

---

## Phase 4B: JWT Middleware & Protected Endpoints

### What You're Learning
- **Middleware** — Request pipeline
- **Authorization** — Restricting access
- **Claims Extraction** — Getting user info from token

### Key Files
- **Program.cs** — Middleware setup and protected endpoints

### Concepts Explained

#### 1. Authentication Middleware
```csharp
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true
    };
});
```

**What this does:**
- Registers JWT as authentication scheme
- Sets validation rules
- Checks signature, issuer, audience, expiration

#### 2. Request Pipeline
```
Request
  ↓
Authentication Middleware (extracts & validates token)
  ↓
Authorization Middleware (checks permissions)
  ↓
Your Endpoint Handler
  ↓
Response
```

**Key order:** Authentication before Authorization

#### 3. Protected Endpoint
```csharp
app.MapGet("/player/stats", async (System.Security.Claims.ClaimsPrincipal user, 
    IPlayerService playerService) =>
{
    var playerIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (!Guid.TryParse(playerIdClaim, out var playerId))
    {
        return Results.Unauthorized();
    }
    
    var statistics = await playerService.GetPlayerStatisticsAsync(playerId);
    return Results.Ok(statistics);
})
.RequireAuthorization();
```

**What happens:**
1. `.RequireAuthorization()` = requires valid token
2. `ClaimsPrincipal user` = injected automatically
3. `FindFirst()` = extract claim by type
4. Use `playerId` to fetch that player's data

#### 4. Using the Token
```
Client Request:
GET /player/stats
Authorization: Bearer eyJhbGc...

Server:
1. Extracts "eyJhbGc..." from header
2. Validates signature with secret key
3. Checks expiration, issuer, audience
4. Extracts claims (playerId, username)
5. Creates ClaimsPrincipal from claims
6. Injects into handler
7. Handler uses playerId to fetch data
```

---

## How It All Connects

```
1. User calls POST /auth/register
   ↓
2. AuthService hashes password, creates Player, generates JWT
   ↓
3. Returns JWT token to client
   ↓
4. Client stores token
   ↓
5. Client calls GET /player/stats with "Authorization: Bearer <token>"
   ↓
6. Middleware validates token signature & claims
   ↓
7. Extracts playerId from claims
   ↓
8. PlayerService queries database using playerId
   ↓
9. Returns that player's statistics
```

---

## Testing the Flow

```bash
# 1. Register
curl -X POST http://localhost:5120/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username": "player1", "password": "password123"}'

# Response: { "playerId": "123-456", "token": "eyJhbGc..." }

# 2. Use token
curl http://localhost:5120/player/stats \
  -H "Authorization: Bearer eyJhbGc..."

# Response: Player statistics
```

---

## .NET Concepts Cheat Sheet

| Concept | What It Does |
|---------|--------------|
| **DI Container** | Automatically creates & injects dependencies |
| **Service** | Encapsulates business logic |
| **DbContext** | Represents database, manages entities |
| **Migration** | Version control for database schema |
| **Async/Await** | Non-blocking operations |
| **LINQ** | Query objects like SQL |
| **Middleware** | Intercepts requests in pipeline |
| **Claims** | User identity information |
| **JWT** | Stateless authentication token |
| **Hashing** | One-way encryption (passwords) |

---

## Next Learning Steps

1. **Understand each layer:**
   - API Layer (endpoints)
   - Service Layer (business logic)
   - Data Layer (database)

2. **Master LINQ:**
   - `.Where()`, `.Select()`, `.FirstOrDefault()`
   - Query syntax vs method syntax

3. **Explore EF Core:**
   - Relationships (one-to-many, many-to-many)
   - Eager loading with `.Include()`
   - Query optimization

4. **Security deepdive:**
   - Encryption vs hashing
   - Refresh tokens
   - Role-based authorization

5. **Testing:**
   - Unit tests with xUnit
   - Mock services
   - Integration tests
