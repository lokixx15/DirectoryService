namespace SharedKernel;

public static class GeneralErrors
{
    public static Error ValueIsNullOrWhitespace(string? name = null) =>
        Error.Validation(
            "value.is.empty.or.whitespace", 
            $"{name ?? "Value"} cannot be empty or whitespace", 
            name ?? "Value");

    public static Error ValueLengthIsNotValid(int maxLength, string? name = null, int minLength = default) =>
        Error.Validation(
            "value.length.is.not.valid", 
            $"{name ?? "Value"} can be from {minLength} to {maxLength} characters long.",
            name ?? "Value");

    public static Error ValueIsNotValid(string message, string? name = null) => 
        Error.Validation("value.is.not.valid", message, name ?? "Value");

    public static Error ValueAlreadyExists(string message) =>
        Error.Conflict("value.already.exists", message);

    public static Error DatabaseInsertFailed(string message, string? code = null) =>
        Error.Failure(code ?? "database.insert.failed", message);

    public static Error OperationCancelled() =>
        Error.Failure("database.operation.cancelled", "Operation was cancelled");
}
