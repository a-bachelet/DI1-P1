namespace Server.Models;

public class Round(int order)
{
    public int? Id { get; private set;}

    public int Order { get; init; } = order;

    public ICollection<RoundAction> Actions { get; } = [];
}
