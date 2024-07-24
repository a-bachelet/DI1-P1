namespace Server.Models;

public enum GameStatus
{
    Waiting,
    InProgress,
    Finished
}

public class Game(string name, int rounds = 15)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public int Rounds { get; set; } = rounds;

    public GameStatus Status { get; set; } = GameStatus.Waiting;

    public ICollection<Player> Players { get; } = [];

    public bool CanBeJoined()
    {
        return Players.Count < 3;
    }

    public bool CanBeStarted()
    {
        return Status == GameStatus.Waiting;
    }
}
