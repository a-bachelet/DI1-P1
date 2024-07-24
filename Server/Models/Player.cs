namespace Server.Models;

public class Player(string name, int gameId)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public int GameId { get; set; } = gameId;

    public Game Game { get; set; } = null!;
}
