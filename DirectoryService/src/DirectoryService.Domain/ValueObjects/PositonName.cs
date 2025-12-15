using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record PositonName
{
    private PositonName(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<PositonName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PositonName>("Name cannot be empty or whitespace.");
        if (value.Length > Constants.MAX_POSITION_NAME_LENGTH || value.Length < Constants.MIN_NAME_LENGTH)
            return Result.Failure<PositonName>("Name can be from 3 to 100 characters long.");

        var name = new PositonName(value);
        return Result.Success(name);
    }
}

