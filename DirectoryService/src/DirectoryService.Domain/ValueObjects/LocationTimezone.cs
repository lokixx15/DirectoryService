using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record LocationTimezone
{
    private LocationTimezone(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<LocationTimezone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            return Result.Failure<LocationTimezone>("Timezone cannot be empty.");

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(value, out _))
            return Result.Failure<LocationTimezone>("Timezone is not valid.");


        var timezone = new LocationTimezone(value);
        return Result.Success(timezone);
    }
}

