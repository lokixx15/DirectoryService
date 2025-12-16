using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record DepartmentPath
{
    private DepartmentPath(
        string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<DepartmentPath> Create(string value)
    {
        if (value.Length > Constants.MAX_DEPARTMENT_PATH_LENGTH)
            return Result.Failure<DepartmentPath>("Path cannot be longer than 300 characters.");

        var path = new DepartmentPath(value);
        return Result.Success(path);
    }
}

