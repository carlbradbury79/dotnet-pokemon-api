# Copilot Instructions for Pokemon Wordle API

## Project Overview

**Goal**: Build a Pokemon-themed Wordle game API with user authentication, game mechanics, leaderboards, and AWS deployment.

**Architecture**:

- .NET 10 Web API with ASP.NET Core
- PostgreSQL database (Docker for local development)
- Lesson-based learning structure
- Clean code, minimal comments, self-documenting names

**Status**:

- ✅ Lesson 1 Complete: Database foundation (5 entities, migrations, EF Core setup)
- 🔄 Lesson 2 In Progress: Game logic & unit testing

---

## Code Conventions

### Namespaces

```csharp
namespace pokemonApi.Models;          // Domain entities
namespace pokemonApi.Data;            // DbContext, repositories
namespace pokemonApi.Services;        // Business logic
namespace pokemonApi.Settings;        // Configuration models
```

### Naming Patterns

- **Models**: PascalCase, singular (User, Game, Guess, not Users/Games)
- **Services**: Interface first, then implementation (IPokemonService, PokemonService)
- **Methods**: Verb-noun, async methods end with `Async` (GetPokemonAsync, CreateGameAsync)
- **Properties**: PascalCase, self-explanatory (UserCount not UC, IsActive not Active)

### Entity Relationships

- Always use navigation properties for EF Core relationships
- Foreign keys use `{EntityName}Id` pattern (UserId, GameId)
- Use cascade delete strategically (User.Games deletes when user deleted)

### Branching Strategy

- `main`: Production-ready code
- `feature/{n}-{description}`: Feature branches for each lesson

---

## Lesson Roadmap

### ✅ Lesson 1: Database Foundation (COMPLETE)

**Completed**:

- 5 domain models (User, Game, Guess, DailyPokemon, Leaderboard)
- EF Core DbContext with Fluent API configuration
- Initial migration (InitialCreate)
- PostgreSQL + Docker Compose setup
- Connection strings in appsettings.json

**Key Learning**: Understanding database relationships, constraints, and how EF Core maps C# to SQL.

---

### 🔄 Lesson 2: Game Logic & Unit Testing (IN PROGRESS)

**Objectives**:

1. Implement `IGuessValidator` interface for Wordle letter-matching logic
2. Write parameterized unit tests using xUnit `[Theory]` and `[InlineData]`
3. Learn edge cases: repeated letters, partial matches, performance
4. Guard clauses and input validation

**What You'll Learn**:

- **Business Logic**: How to determine letter status (green/yellow/gray)
- **Unit Testing**: Test-driven development, mocking, parameterized tests
- **Edge Cases**: Handling tricky Wordle scenarios (duplicate letters in different positions)
- **Code Quality**: Guard clauses prevent bugs, make code readable

**Deliverables**:

- `Services/IGuessValidator.cs` - Interface defining letter validation
- `Services/GuessValidator.cs` - Implementation with algorithm
- `tests/pokemonApi.UnitTests/Services/GuessValidatorTests.cs` - Comprehensive test cases

---

### Lesson 3: User Authentication & JWT

- User registration/login endpoints
- JWT token generation and validation
- Password hashing with BCrypt (already installed)
- Authorization middleware

### Lesson 4: Game Session Management

- Create game endpoint
- Submit guess endpoint
- Win/lose conditions
- Time tracking

### Lesson 5: Leaderboard & Statistics

- Aggregate leaderboard queries
- User statistics endpoint
- Performance optimization

### Lesson 6: Daily Pokemon Selection

- Scheduled task to select daily Pokemon
- Seeded randomization
- Background jobs

### Lesson 7: AWS Deployment

- Deploy to AWS App Runner
- RDS PostgreSQL setup
- Environment configuration
- CI/CD with GitHub Actions

---

## Teaching Style

When you ask for help, I will:

1. **Explain the "why"** - Reasoning behind patterns and approaches
2. **Show working examples** - Code you can study and adapt
3. **Break down complexity** - Step-by-step explanations
4. **Give hands-on instructions** - Exactly what to type and why
5. **Connect to existing code** - Relate new concepts to what you've already learned
6. **Encourage best practices** - Clean code, testing, maintainability
7. **Link to documentation** - when there is an option for further reading to dive deeper

---

## Code Quality Standards

### ✅ Do This

- Clean variable names that explain intent
- Guard clauses to fail fast (validate inputs early)
- Dependency injection for testability
- Async/await for I/O operations
- One responsibility per class/method

### ❌ Don't Do This

- Over-comment simple code (method names should explain intent)
- Premature optimization (make it work, then optimize)
- Large methods trying to do too much
- Hardcoded values (use configuration)
- Ignore edge cases

### Example - Guard Clause Pattern

```csharp
// ❌ Avoid: Deeply nested, hard to follow
public bool ValidateGuess(string guessed, string answer)
{
    if (guessed != null)
    {
        if (guessed.Length > 0)
        {
            if (answer != null)
            {
                // business logic
            }
        }
    }
}

// ✅ Prefer: Guard clauses fail fast, readable
public bool ValidateGuess(string guessed, string answer)
{
    if (string.IsNullOrEmpty(guessed)) return false;
    if (string.IsNullOrEmpty(answer)) return false;

    // business logic now
}
```

---

## How to Use This File

1. **Reference in questions**: "Following the lesson roadmap..."
2. **Ask for clarification**: "Explain how to apply the guard clause pattern to..."
3. **Challenge yourself**: "Using the code quality standards, what's wrong with this code?"
4. **Track progress**: Each lesson adds to your skills - they build on each other

---

## Key Concepts You'll Master

By the end of this project, you'll understand:

- ✅ **Database Design**: Relationships, constraints, migrations, EF Core
- 🔄 **Game Algorithm** (Lesson 2): Letter matching, edge cases, testing
- 🔄 **Authentication** (Lesson 3): JWT, hashing, security
- 🔄 **API Design** (Lessons 4-6): RESTful endpoints, status codes, error handling
- 🔄 **Deployment** (Lesson 7): Cloud hosting, environment management, CI/CD

---

## Getting Help

When you're stuck, tell me:

1. **What you're trying to do** - The goal of this step
2. **What you've tried** - Code or approach you attempted
3. **What went wrong** - Error message or unexpected behavior
4. **What you understand** - What makes sense vs. what's confusing

This helps me give targeted, deep explanations instead of generic answers.
