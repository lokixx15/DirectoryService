using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public class LocationTimezone
{
    private LocationTimezone(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<LocationTimezone> Create(string value)
    {
        var timezone = new LocationTimezone(value);

        return Result.Success(timezone);
    }
}

