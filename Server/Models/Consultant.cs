using Server.Hubs.Records;

namespace Server.Models;

public class Consultant(string name, int salaryRequirement, int gameId)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public ICollection<LeveledSkill> Skills { get; } = [];

    public int SalaryRequirement { get; set; } = salaryRequirement;

    public int GameId { get; set; } = gameId;

    public Game Game { get; set; } = null!;

    public ConsultantOverview ToOverview()
    {
        return new ConsultantOverview(
            Id is null ? 0 : (int) Id, Name,
            SalaryRequirement, Skills.Select(s => s.ToOverview()).ToList()
        );
    }
}
