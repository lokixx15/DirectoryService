namespace SharedKernel;

public static class GeneralErrors
{
    public static Error ValueIsNullOrWhitespace(string? name = null)
    {
        string valueName = name ?? "Value";

        return Error.Validation(
            "value.is.empty.or.whitespace",
            $"{valueName} cannot be empty or whitespace",
            valueName);

    }

    public static Error ValueLengthIsNotInvalid(int maxLength, string? name = null, int minLength = default)
    {
        string valueName = name ?? "Value";

        return Error.Validation(
            "value.length.is.not.valid",
            $"{valueName} can be from {minLength} to {maxLength} characters long.",
            valueName);
    }

    public static Error ValueIsNotValid(string message, string? name = null)
    {
        string valueName = name ?? "Value";
        
        return Error.Validation(
            "value.is.not.valid",
            message,
            valueName);

    }

    public static Error InsertFailed(string message, string? code = null)
    {
        return Error.Failure(code, message);
    }
}
