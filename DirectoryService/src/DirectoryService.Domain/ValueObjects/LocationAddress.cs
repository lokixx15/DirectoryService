using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.ValueObjects;

public record LocationAddress
{
    private LocationAddress(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<LocationAddress, Errors> Create(string value)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationAddress, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Address"));

        if (value.Length > Constants.MAX_LOCATION_ADDRESS_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotInvalid(Constants.MAX_LOCATION_ADDRESS_LENGTH, "Address"));

        if (errors.Any())
            return Result.Failure<LocationAddress, Errors>(errors);

        var address = new LocationAddress(value);

        return Result.Success<LocationAddress, Errors>(address);
    }
}