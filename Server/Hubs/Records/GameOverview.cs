namespace Server.Hubs.Records;

public sealed record GameOverview(
    int Id,
    string Name,
    ICollection<PlayerOverview> Players
);

public sealed record PlayerOverview(
    int Id,
    string Name
);

