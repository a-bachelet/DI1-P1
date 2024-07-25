namespace Server.Models;

public class Company(string name, int playerId)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public int PlayerId { get; set; } = playerId;

    public Player Player { get; set; } = null!;

    public ICollection<Employee> Employees { get; } = [];
}
