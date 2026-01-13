using CSharpFunctionalExtensions;
using SharedKernel;
using System.Text.RegularExpressions;

namespace DirectoryService.Domain.Departments.VO;

public record DepartmentIdentifier
{
    //ef core 
    private DepartmentIdentifier() { }

    private DepartmentIdentifier(
        string value)
    {
        Value = value;
    }

    public string Value { get; } = string.Empty;

    public static Result<DepartmentIdentifier, Errors> Create(string value)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentIdentifier, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Identifier"));

        if (value.Length > Constants.MAX_DEPARTMENT_IDENTIFIER_LENGTH || value.Length < Constants.MIN_NAME_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotValid(Constants.MAX_DEPARTMENT_IDENTIFIER_LENGTH, "Identifier", Constants.MIN_NAME_LENGTH));

        if (!Regex.IsMatch(value, Constants.IS_LATIN_PATTERN))
            errors.Add(GeneralErrors.ValueIsNotValid("Identifier must be in Latin", "Identifier"));

        if (errors.Any())
            return Result.Failure<DepartmentIdentifier, Errors>(errors);

        var name = new DepartmentIdentifier(value);

        return Result.Success<DepartmentIdentifier, Errors>(name);
    }
}
