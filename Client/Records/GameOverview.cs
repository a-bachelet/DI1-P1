namespace Client.Records;

public sealed record GameOverview(
    int Id,
    string Name,
    ICollection<PlayerOverview> Players,
    int PlayersCount,
    int MaximumPlayersCount
);

public sealed record PlayerOverview(
    int Id,
    string Name
);
