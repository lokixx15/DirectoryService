using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentPath
{
    private DepartmentPath(
        string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<DepartmentPath, Errors> Create(string value)
    {
        var errors = new List<Error>();

        if (value.Length > Constants.MAX_DEPARTMENT_PATH_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotInvalid(Constants.MAX_DEPARTMENT_PATH_LENGTH, "Path"));

        if (errors.Any())
            return Result.Failure<DepartmentPath, Errors>(errors);

        var path = new DepartmentPath(value);

        return Result.Success<DepartmentPath, Errors>(path);
    }
}

