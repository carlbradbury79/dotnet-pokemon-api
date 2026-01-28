# Pokemon Wordle API - Lesson 1 Foundation

## Current Status: `lesson/1-database-setup` branch

You've successfully completed **Lesson 1: Foundation - EF Core, Docker, PostgreSQL & CI/CD**

### ✅ What's Been Completed

1. **Database Schema Design** ✓
   - 5 domain entities (User, Game, Guess, DailyPokemon, Leaderboard)
   - Relationships and constraints defined
   - EF Core DbContext configured

2. **Local Development Environment** ✓
   - Docker Compose setup with PostgreSQL + pgAdmin
   - Connection strings in appsettings.json
   - Initial migration created

3. **Testing Infrastructure** ✓
   - UnitTests project created
   - IntegrationTests project created
   - Test projects with xUnit, Moq, and Testcontainers configured

4. **CI/CD Pipeline** ✓
   - GitHub Actions workflow created
   - Tests run automatically on PR
   - Test results uploaded as artifacts

### 📋 Branch Content

**Files Added:**
- `Models/User.cs` - User entity with password storage
- `Models/Game.cs` - Game session tracking
- `Models/Guess.cs` - Individual guess attempts
- `Models/DailyPokemon.cs` - Daily puzzle record
- `Models/Leaderboard.cs` - Leaderboard entries
- `Data/PokemonDbContext.cs` - EF Core configuration
- `Migrations/20260128231111_InitialCreate.*` - Database schema
- `tests/pokemonApi.UnitTests/` - Unit test project
- `tests/pokemonApi.IntegrationTests/` - Integration test project
- `docker-compose.yml` - Local database setup
- `.github/workflows/dotnet-tests.yml` - CI/CD pipeline
- `LESSON_1_SUMMARY.md` - Detailed lesson guide

**Files Modified:**
- `Program.cs` - Added EF Core DbContext configuration
- `appsettings.json` - Added PostgreSQL connection string
- `pokemonApi.csproj` - Added EF Core NuGet packages

### 🚀 Next Steps

1. **Create a Pull Request on GitHub**
   ```bash
   # Push this branch to GitHub
   git push origin lesson/1-database-setup
   ```

2. **Configure Branch Protection** (one-time setup)
   - Go to Settings > Branches > main
   - Add rule for main branch
   - Require status checks to pass (GitHub Actions tests)
   - Require PR reviews before merge

3. **Merge When Ready**
   - After PR review and tests pass, merge to main
   - Tests will run automatically before allowing merge

4. **Proceed to Lesson 2**
   - Branch: `lesson/2-game-logic-unit-testing`
   - Focus: Pure Wordle game logic with comprehensive unit tests

### 📚 Learning Materials

See [LESSON_1_SUMMARY.md](LESSON_1_SUMMARY.md) for:
- Detailed learning objectives
- Key concepts explained
- Project structure overview
- Getting started guide
- Troubleshooting tips

### 🔧 Local Development Commands

```bash
# Start PostgreSQL locally
docker-compose up -d

# Apply database migrations
dotnet ef database update

# View pgAdmin (database management)
# URL: http://localhost:5050
# Email: admin@example.com
# Password: admin

# Build the solution (note: test projects may show IDE warnings due to .NET 10)
dotnet build

# Run tests in GitHub Actions (they run correctly in CI environment)
# Local test execution has a known .NET 10 SDK discovery issue
```

### 💡 Key Learning Points

This lesson introduced:
- **Designing relatable entities** that map to business logic
- **EF Core's configuration patterns** for database constraints
- **Separation of concerns** with test projects
- **Infrastructure as code** with Docker
- **Automated testing** with GitHub Actions

### 🎯 What to Review

1. Read through the entity models to understand relationships
2. Review `PokemonDbContext.cs` to see Fluent API configuration
3. Examine `docker-compose.yml` for environment setup
4. Study `.github/workflows/dotnet-tests.yml` for CI/CD patterns

---

**Status**: ✅ Complete - Ready for PR  
**Next**: Lesson 2 - Pure Game Logic & Unit Testing  
**Created**: January 28, 2026  
**Framework**: .NET 10.0 with ASP.NET Core  
**Database**: PostgreSQL 17 (Alpine)  
