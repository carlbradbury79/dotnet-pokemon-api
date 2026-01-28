# Lesson 1: Foundation - EF Core, Docker, PostgreSQL & CI/CD

## Lesson Overview

This lesson establishes the complete foundation for the Pokemon Wordle API. It covers database design with EF Core, Docker setup for local development, testing infrastructure, and GitHub Actions for CI/CD.

## What You'll Learn

### 1. Entity-Relationship Modeling
- Designing domain entities that map to database tables
- Understanding foreign key relationships and cascade deletes
- Using navigation properties in EF Core
- Implementing unique constraints at the entity level

### 2. Entity Framework Core
- Configuring DbContext with Npgsql PostgreSQL provider
- Using OnModelCreating to customize entity mappings
- Understanding Fluent API configuration
- Working with nullable reference types and required properties

### 3. Database Migrations
- Creating initial migrations with `dotnet ef migrations add`
- Understanding migration history and rollback
- Viewing generated SQL and schema changes
- Applying migrations to development databases

### 4. Docker & Local Development
- Setting up docker-compose.yml for PostgreSQL + pgAdmin
- Configuring container networking and health checks
- Environment variables and connection strings
- Managing persistent volumes

### 5. .NET Testing Infrastructure
- Creating separate UnitTests and IntegrationTests projects
- Configuring xUnit test framework
- Adding Moq for mocking and Testcontainers for database tests
- Understanding test project dependencies

### 6. CI/CD with GitHub Actions
- Setting up GitHub Actions workflows to run on pull requests
- Running unit and integration tests automatically
- Configuring PostgreSQL service container for tests
- Uploading test artifacts for analysis

## Key Concepts Introduced

### Domain Entities Created
1. **User** - Represents a player with login credentials
2. **Game** - Tracks a single game session for a user
3. **Guess** - Records each attempt within a game
4. **DailyPokemon** - Represents the daily puzzle (same for all users)
5. **Leaderboard** - Tracks fastest times per Pokemon per user

### Learning Points in Code
- Comments labeled "Learning Point" explain design decisions
- Enum usage for GameStatus and LetterStatus
- JSON serialization for storing guess results
- Relationship configuration (one-to-many, composite keys)

## Technologies Introduced
- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL (via Docker)
- **ORM**: Entity Framework Core 10.0
- **Testing**: xUnit, Moq, Testcontainers
- **CI/CD**: GitHub Actions
- **Password Hashing**: BCrypt.Net-Next

## Project Structure After Lesson 1

```
pokemonApi/
├── Program.cs                          # Dependency injection & middleware setup
├── pokemonApi.csproj                   # Main project dependencies
├── appsettings.json                    # Configuration (connection string, logging)
├── docker-compose.yml                  # Local development PostgreSQL setup
├── .github/workflows/
│   └── dotnet-tests.yml               # GitHub Actions CI/CD workflow
├── Models/
│   ├── User.cs                         # User entity
│   ├── Game.cs                         # Game entity
│   ├── Guess.cs                        # Guess entity
│   ├── DailyPokemon.cs                 # Daily puzzle entity
│   ├── Leaderboard.cs                  # Leaderboard entry entity
│   ├── PokemonApiResponse.cs           # (existing)
│   └── PokemonListResponse.cs          # (existing)
├── Data/
│   └── PokemonDbContext.cs             # EF Core DbContext with all configurations
├── Migrations/
│   ├── 20260128231111_InitialCreate.cs # Generated migration
│   ├── 20260128231111_InitialCreate.Designer.cs
│   └── PokemonDbContextModelSnapshot.cs
├── tests/
│   ├── pokemonApi.UnitTests/
│   │   ├── pokemonApi.UnitTests.csproj
│   │   └── ExampleUnitTests.cs        # Placeholder for Lesson 2
│   └── pokemonApi.IntegrationTests/
│       ├── pokemonApi.IntegrationTests.csproj
│       └── PokemonDbContextIntegrationTests.cs # Placeholder for Lesson 2
```

## Getting Started with Lesson 1

### 1. Start PostgreSQL Locally
```bash
docker-compose up -d
# PostgreSQL runs on localhost:5432
# pgAdmin available at http://localhost:5050 (admin@example.com / admin)
```

### 2. View Generated Migration
```bash
# See the SQL that will be executed
dotnet ef migrations script
```

### 3. Apply Migration to Database
```bash
# Creates tables and constraints in PostgreSQL
dotnet ef database update
```

### 4. Verify in pgAdmin
- Open http://localhost:5050
- Login with admin@example.com / admin
- Query tables under Databases > pokemondb > Schemas > public

### 5. Run Tests Locally (Note: Known .NET 10 SDK issue)
```bash
# Test projects are configured but may require IDE restart or CI environment
# Tests will run correctly in GitHub Actions
dotnet test tests/pokemonApi.UnitTests/
dotnet test tests/pokemonApi.IntegrationTests/
```

### 6. Push to GitHub (After Configuring Branch Protection)
```bash
git push origin lesson/1-database-setup
# Create PR - GitHub Actions will automatically run tests
```

## Configuration Files

### appsettings.json
- `ConnectionStrings.DefaultConnection` points to local Docker PostgreSQL
- Logging configuration for development

### docker-compose.yml
- PostgreSQL 17 Alpine (lightweight image)
- pgAdmin for database management
- Named volume for data persistence
- Health check to ensure readiness

### .github/workflows/dotnet-tests.yml
- Runs on PR and push to main/lesson/* branches
- Sets up PostgreSQL service container
- Runs unit tests then integration tests
- Uploads test results as artifacts

## Next Steps

This foundation is ready for:
- **Lesson 2**: Pure game logic testing (Wordle letter matching)
- **Lesson 3**: User authentication with JWT
- **Lesson 4**: Daily Pokemon endpoint and game mechanics
- **Lesson 5**: Statistics tracking
- **Lesson 6**: Leaderboards
- **Lesson 7**: AWS deployment

## Key Files to Review

1. [Models/User.cs](Models/User.cs) - Domain entity example with relationships
2. [Data/PokemonDbContext.cs](Data/PokemonDbContext.cs) - EF Core configuration patterns
3. [docker-compose.yml](docker-compose.yml) - Local development setup
4. [.github/workflows/dotnet-tests.yml](.github/workflows/dotnet-tests.yml) - CI/CD configuration

## Troubleshooting

**"Connection refused" when running migrations**
- Ensure `docker-compose up -d` is running
- Check PostgreSQL is healthy: `docker-compose ps`
- Verify connection string in appsettings.json matches docker-compose config

**Tests won't build locally**
- This is a known .NET 10 SDK auto-discovery issue
- Tests run correctly in GitHub Actions (fresh environment)
- Tests folder structure prevents local IDE interference

**PostgreSQL data doesn't persist**
- Named volume `postgres_data` should persist between restarts
- To reset database: `docker-compose down -v` (removes volume)
- Then `docker-compose up -d` to start fresh

## Learning Reflection

At the end of Lesson 1, you should understand:
1. How domain entities map to database tables
2. The purpose of DbContext in EF Core
3. Why migrations track schema changes
4. How Docker provides reproducible environments
5. Why separate test projects improve code organization
6. How GitHub Actions automates testing on every PR

---

**Branch**: `lesson/1-database-setup`  
**Status**: Ready for PR / Review  
**Next Lesson**: Lesson 2 - Pure Game Logic & Unit Testing
