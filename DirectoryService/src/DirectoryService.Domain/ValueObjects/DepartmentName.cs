using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentName
{
    private DepartmentName(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<DepartmentName, Errors> Create(string value)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentName, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Name"));

        if (value.Length > Constants.MAX_DEPARTMENT_NAME_LENGTH || value.Length < Constants.MIN_NAME_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotInvalid(Constants.MAX_DEPARTMENT_PATH_LENGTH, "Name", Constants.MIN_NAME_LENGTH));

        if (errors.Any())
            return Result.Failure<DepartmentName, Errors>(errors);

        var name = new DepartmentName(value);

        return Result.Success<DepartmentName, Errors>(name);
    }
}

