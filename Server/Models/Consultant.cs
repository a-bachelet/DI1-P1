namespace Server.Models;

public class Consultant(string name, int salaryRequirement)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public ICollection<ConsultantSkill> Skills { get; } = [];

    public int SalaryRequirement { get; set; } = salaryRequirement;
}
