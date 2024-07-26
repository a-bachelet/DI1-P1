namespace Server.Models;

public class Skill(string name)
{
    public int? Id { get; init; }

    public string Name { get; set; } = name;
}
