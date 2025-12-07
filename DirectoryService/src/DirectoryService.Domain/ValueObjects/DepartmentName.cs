using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public class DepartmentName
{
    private const int MAX_NAME_LENGTH = 150;

    private DepartmentName(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<DepartmentName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<DepartmentName>("Name cannot be empty or whitespace.");
        if (value.Length > MAX_NAME_LENGTH || value.Length < Constants.MIN_NAME_LENGTH)
            return Result.Failure<DepartmentName>("Name can be from 3 to 150 characters long.");

        var name = new DepartmentName(value);
        return Result.Success(name);
    }
}

