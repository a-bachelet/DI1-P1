namespace Server.Models;

public class Employee(string name, int companyId)
{
    public int? Id { get; private set; }

    public string Name { get; set; } = name;

    public int CompanyId { get; set; } = companyId;

    public Company Company { get; set; } = null!;
}
