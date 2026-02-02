using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pokemonApi.Data;
using pokemonApi.Models;
using pokemonApi.Services;
using pokemonApi.Settings;
using Xunit;

namespace pokemonApi.UnitTests.Services;

public class AuthServiceTests
{
    private readonly PokemonDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<PokemonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PokemonDbContext(options);
        
        var jwtSettings = new JwtSettings
        {
            SecretKey = "test-secret-key-at-least-32-characters-long-for-testing",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationMinutes = 60
        };

        _tokenService = new TokenService(jwtSettings);
        _authService = new AuthService(_context, _tokenService);
    }

    #region Registration Tests

    [Fact]
    public async Task RegisterAsync_WithValidCredentials_ReturnsSuccessAndToken()
    {
        var request = new RegisterRequest("testuser", "test@example.com", "password123");
        
        var result = await _authService.RegisterAsync(request);

        Assert.True(result.Success);
        Assert.NotNull(result.Token);
        Assert.Equal("User registered successfully.", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ReturnsFails()
    {
        var request = new RegisterRequest("testuser", "test@example.com", "password123");
        await _authService.RegisterAsync(request);

        var duplicateRequest = new RegisterRequest("testuser", "another@example.com", "password123");
        var result = await _authService.RegisterAsync(duplicateRequest);

        Assert.False(result.Success);
        Assert.Null(result.Token);
        Assert.Equal("Username already exists.", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_WithShortPassword_ReturnsFails()
    {
        var request = new RegisterRequest("testuser", "test@example.com", "pass");
        
        var result = await _authService.RegisterAsync(request);

        Assert.False(result.Success);
        Assert.Null(result.Token);
        Assert.Equal("Password must be at least 6 characters.", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_WithEmptyUsername_ReturnsFails()
    {
        var request = new RegisterRequest("", "test@example.com", "password123");
        
        var result = await _authService.RegisterAsync(request);

        Assert.False(result.Success);
        Assert.Null(result.Token);
        Assert.Equal("Username and password are required.", result.Message);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessAndToken()
    {
        var registerRequest = new RegisterRequest("testuser", "test@example.com", "password123");
        await _authService.RegisterAsync(registerRequest);

        var loginRequest = new LoginRequest("testuser", "password123");
        var result = await _authService.LoginAsync(loginRequest);

        Assert.True(result.Success);
        Assert.NotNull(result.Token);
        Assert.Equal("Login successful.", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFails()
    {
        var registerRequest = new RegisterRequest("testuser", "test@example.com", "password123");
        await _authService.RegisterAsync(registerRequest);

        var loginRequest = new LoginRequest("testuser", "wrongpassword");
        var result = await _authService.LoginAsync(loginRequest);

        Assert.False(result.Success);
        Assert.Null(result.Token);
        Assert.Equal("Invalid username or password.", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithNonexistentUser_ReturnsFails()
    {
        var loginRequest = new LoginRequest("nonexistent", "password123");
        var result = await _authService.LoginAsync(loginRequest);

        Assert.False(result.Success);
        Assert.Null(result.Token);
        Assert.Equal("Invalid username or password.", result.Message);
    }

    #endregion

    #region Token Service Tests

    [Fact]
    public void GenerateToken_CreatesValidToken()
    {
        var token = _tokenService.GenerateToken(1, "testuser");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsUserId()
    {
        var token = _tokenService.GenerateToken(1, "testuser");
        
        var userId = _tokenService.ValidateToken(token);

        Assert.NotNull(userId);
        Assert.Equal(1, userId);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsNull()
    {
        var invalidToken = "invalid.token.here";
        
        var userId = _tokenService.ValidateToken(invalidToken);

        Assert.Null(userId);
    }

    #endregion
}
