using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public class LocationName
{
    private const int MAX_NAME_LENGTH = 120;

    private LocationName(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<LocationName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationName>("Name cannot be empty or whitespace.");
        if (value.Length > MAX_NAME_LENGTH || value.Length < Constants.MIN_NAME_LENGTH)
            return Result.Failure<LocationName>("Name can be from 3 to 120 characters long.");

        var name = new LocationName(value);
        return Result.Success(name);
    }
}

