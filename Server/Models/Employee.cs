using Server.Hubs.Records;

namespace Server.Models;

public class Employee(string name, int? companyId, int gameId, int salary) : Consultant(name, salary, gameId)
{
    public int? CompanyId { get; set; } = companyId;

    public Company Company { get; set; } = null!;

    public int Salary { get; set; } = salary;

    public new EmployeeOverview ToOverview()
    {
        return new EmployeeOverview(
            Id is null ? 0 : (int) Id, Name, SalaryRequirement,
            Salary, Skills.Select(s => s.ToOverview()).ToList()
        );
    }
}
