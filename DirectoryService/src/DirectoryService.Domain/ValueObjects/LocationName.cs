using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.ValueObjects;

public record LocationName
{
    private LocationName(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<LocationName, Errors> Create(string value)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<LocationName, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Name"));

        if (value.Length > Constants.MAX_LOCATION_NAME_LENGTH || value.Length < Constants.MIN_NAME_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotInvalid(Constants.MAX_LOCATION_NAME_LENGTH, "Name", Constants.MIN_NAME_LENGTH));

        if (errors.Any())
            return Result.Failure<LocationName, Errors>(errors);

        var name = new LocationName(value);

        return Result.Success<LocationName, Errors>(name);
    }
}