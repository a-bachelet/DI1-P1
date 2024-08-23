namespace Server.Models;

public class Employee(string name, int companyId, int gameId, int salary) : Consultant(name, gameId, salary)
{
    public int CompanyId { get; set; } = companyId;

    public Company Company { get; set; } = null!;

    public new ICollection<EmployeeSkill> Skills { get; } = [];

    public int Salary { get; set; } = salary;
}
