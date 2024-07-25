namespace Server.Models;

public sealed class EmployeeSkill(string name, int level)
{
    public string Name { get; set; } = name;

    public int Level { get; set; } = level;
}

