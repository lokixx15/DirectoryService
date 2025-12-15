using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public record LocationAddress
{
    private LocationAddress(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<LocationAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationAddress>("Address cannot be empty or whitespace.");
        if (value.Length > Constants.MAX_LOCATION_ADDRESS_LENGTH)
            return Result.Failure<LocationAddress>("Address cannot be longer than 200 characters.");

        var address = new LocationAddress(value);
        return Result.Success(address);
    }
}

