// Learning Point: This is a domain entity representing a user in our system.
// EF Core will map this to the "Users" table. Note: we use 'Id' as primary key by convention.
namespace pokemonApi.Models;

public class User
{
    public int Id { get; set; }
    
    /// <summary>
    /// Username must be unique - will add a unique constraint in migration.
    /// </summary>
    public string Username { get; set; } = null!;
    
    /// <summary>
    /// Password is stored as a bcrypt hash, never store plain text passwords.
    /// </summary>
    public string PasswordHash { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties for EF Core relationships
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
