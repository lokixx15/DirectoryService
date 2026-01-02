using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.ValueObjects;

public record PositonName
{
    private PositonName(
    string value)
    {
        Value = value;
    }

    public string Value { get; private set; } = string.Empty;

    public static Result<PositonName, Errors> Create(string value)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PositonName, Errors>(GeneralErrors.ValueIsNullOrWhitespace("Name"));

        if (value.Length > Constants.MAX_POSITION_NAME_LENGTH || value.Length < Constants.MIN_NAME_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotInvalid(Constants.MAX_POSITION_NAME_LENGTH, "Name", Constants.MIN_NAME_LENGTH));

        if (errors.Any())
            return Result.Failure<PositonName, Errors>(errors);

        return Result.Success<PositonName, Errors>(new PositonName(value));
    }
}

