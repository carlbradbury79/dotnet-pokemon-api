using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using pokemonApi.Data;
using pokemonApi.Models;

namespace pokemonApi.Services;

public class AuthService : IAuthService
{
    private readonly PokemonDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(PokemonDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return new AuthResponse(false, null, "Username and password are required.");

        if (request.Password.Length < 6)
            return new AuthResponse(false, null, "Password must be at least 6 characters.");

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (existingUser != null)
            return new AuthResponse(false, null, "Username already exists.");

        var hashedPassword = BC.HashPassword(request.Password);
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user.Id, user.Username);
        return new AuthResponse(true, token, "User registered successfully.");
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return new AuthResponse(false, null, "Username and password are required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            return new AuthResponse(false, null, "Invalid username or password.");

        var token = _tokenService.GenerateToken(user.Id, user.Username);
        return new AuthResponse(true, token, "Login successful.");
    }
}
