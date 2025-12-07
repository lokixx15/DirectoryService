using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public class DepartmentPath
{
    private DepartmentPath(
        string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<DepartmentPath> Create(string value)
    {
        var path = new DepartmentPath(value);

        return Result.Success(path);
    }
}

