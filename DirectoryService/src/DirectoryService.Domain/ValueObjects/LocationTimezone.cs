using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.ValueObjects;

public record LocationTimezone
{
    private LocationTimezone(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<LocationTimezone, Errors> Create(string value)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationTimezone, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Timezone"));

        if (value.Length > Constants.MAX_LOCATION_TIMEZONE_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotInvalid(Constants.MAX_LOCATION_TIMEZONE_LENGTH, "Timezone"));

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(value, out _))
            errors.Add(GeneralErrors.ValueIsNotValid("Timezone is not valid", "Timezone"));

        if (errors.Any())
                return Result.Failure<LocationTimezone, Errors>(errors);

        var timezone = new LocationTimezone(value);

        return Result.Success<LocationTimezone, Errors>(timezone);
    }
}