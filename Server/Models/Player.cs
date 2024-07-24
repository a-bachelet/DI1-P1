namespace Server.Models;

public class Player(string name, int gameId)
{
  public int Id { get; }

  public string Name { get; } = name;

  public int GameId { get; } = gameId;

  public Game Game { get; } = null!;
}
