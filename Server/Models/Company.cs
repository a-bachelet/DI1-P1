namespace Server.Models;

public class Company(string name, int playerId)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public int PlayerId { get; set; } = playerId;

    public Player Player { get; set; } = null!;

    public int Treasury { get; set; } = 1000000;

    public ICollection<Employee> Employees { get; } = [];
}
