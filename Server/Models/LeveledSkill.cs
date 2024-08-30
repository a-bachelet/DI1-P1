using Server.Hubs.Records;

namespace Server.Models;

public class LeveledSkill(string name, int level)
{
    public string Name { get; set; } = name;

    public int Level { get; set; } = level;

    public SkillOverview ToOverview()
    {
        return new SkillOverview(Name, Level);
    }
}
