namespace Server.Models;

public class Game(string name, int rounds = 15)
{
    public int Id { get; private set; }

    public string Name { get; } = name;

    public int Rounds { get; } = rounds;

    public ICollection<Player> Players { get; } = [];
}
