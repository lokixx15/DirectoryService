using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.ValueObjects;

public class LocationAddress
{
    private const int MAX_ADDRESS_LENGTH = 200;

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
        if (value.Length > MAX_ADDRESS_LENGTH)
            return Result.Failure<LocationAddress>("Address cannot be longer than 200 characters.");

        var address = new LocationAddress(value);

        return Result.Success(address);
    }
}

