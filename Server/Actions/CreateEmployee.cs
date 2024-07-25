
using FluentResults;

using FluentValidation;

using Server.Actions.Contracts;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record CreateEmployeeParams(string EmployeeName, int? CompanyId = null, Company? Company = null);

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeParams>
{
    public CreateEmployeeValidator()
    {
        RuleFor(p => p.EmployeeName).NotEmpty();
        RuleFor(p => p.CompanyId).NotEmpty().When(p => p.Company is null);
        RuleFor(p => p.Company).NotEmpty().When(p => p.CompanyId is null);
    }
}

public class CreateEmployee(
    ICompaniesRepository companiesRepository,
    IEmployeesRepository employeesRepository,
    ISkillsRepository skillsRepository
) : IAction<CreateEmployeeParams, Result<Employee>>
{
    public async Task<Result<Employee>> PerformAsync(CreateEmployeeParams actionParams)
    {
        var rnd = new Random();

        var actionValidator = new CreateEmployeeValidator();
        var actionValidationResult = await actionValidator.ValidateAsync(actionParams);

        if (actionValidationResult.Errors.Count != 0)
        {
            return Result.Fail(actionValidationResult.Errors.Select(e => e.ErrorMessage));
        }

        var (employeeName, companyId, company) = actionParams;

        company ??= await companiesRepository.GetById(companyId!.Value);

        if (company is null)
        {
            Result.Fail($"Company with Id \"{companyId}\" not found.");
        }

        IEnumerable<int> salaries = Array.Empty<int>();

        for (var salary = 29000; salary <= 100000; salary++)
        {
            salaries = salaries.Append(salary);
        }

        var randomSalary = rnd.Next(salaries.Count() + 1);

        var employee = new Employee(employeeName, company!.Id!.Value, randomSalary);

        var randomSkills = await skillsRepository.GetRandomSkills(3);

        foreach (var randomSkill in randomSkills)
        {
            employee.Skills.Add(new EmployeeSkill(randomSkill.Name, rnd.Next(11)));
        }

        await employeesRepository.SaveEmployee(employee);

        return Result.Ok(employee);
    }
}
