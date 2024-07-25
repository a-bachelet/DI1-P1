namespace Server.Models;

public class ConsultantSkill(string name, int level)
{
    public string Name { get; set; } = name;

    public int Level { get; set; } = level;
}
