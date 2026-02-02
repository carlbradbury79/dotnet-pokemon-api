namespace pokemonApi.Services;

public interface ITokenService
{
    /// <summary>Generates a JWT token for the given user ID.</summary>
    string GenerateToken(int userId, string username);
    
    /// <summary>Validates a JWT token and extracts the user ID if valid.</summary>
    int? ValidateToken(string token);
}
