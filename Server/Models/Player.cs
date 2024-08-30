using Server.Hubs.Records;

namespace Server.Models;

public class Player(string name, int gameId)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public int GameId { get; set; } = gameId;

    public Game Game { get; set; } = null!;

    public int? CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public PlayerOverview ToOverview()
    {
        return new PlayerOverview(
            Id is null ? 0 : (int) Id, Name,
            Company.ToOverview()
        );
    }
}
