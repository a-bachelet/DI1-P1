
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
    IEmployeesRepository employeesRepository
) : IAction<CreateEmployeeParams, Result<Employee>>
{
    public async Task<Result<Employee>> PerformAsync(CreateEmployeeParams actionParams)
    {
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

        var employee = new Employee(employeeName, company!.Id!.Value);

        await employeesRepository.SaveEmployee(employee);

        return Result.Ok(employee);
    }
}
