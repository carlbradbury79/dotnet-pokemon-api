namespace pokemonApi.Models;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;
    
    public string PasswordHash { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties for EF Core relationships
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
