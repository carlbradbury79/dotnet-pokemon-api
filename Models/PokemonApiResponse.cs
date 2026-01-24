namespace PokemonApi.Models;

public class PokemonApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TypeSlot> Types { get; set; } = new();
    public Sprites Sprites { get; set; } = new();
    public int Base_experience { get; set; } = 0;
    public List<Abilities> Abilities { get; set; } = new();
    
}

public class TypeSlot
{
    public int Slot { get; set; }
    public TypeInfo Type { get; set; } = new();
}

public class TypeInfo
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class Sprites
{
    public string FrontDefault { get; set; } = string.Empty;
}

public class Abilities
{
    public AbilityInfo Ability { get; set; } = new();
    public bool Is_hidden { get; set; }
    public int Slot { get; set; }
}

public class AbilityInfo
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
