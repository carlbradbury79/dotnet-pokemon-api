namespace pokemonApi.Services;

public record RegisterRequest(string Username, string Email, string Password);

public record LoginRequest(string Username, string Password);

public record AuthResponse(bool Success, string? Token, string? Message);

public interface IAuthService
{
    /// <summary>Registers a new user with username, email, and password.</summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    
    /// <summary>Authenticates user and returns JWT token if credentials are valid.</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
