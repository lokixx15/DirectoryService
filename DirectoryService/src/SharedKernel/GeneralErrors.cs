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

    public static Error EntityNotFound(string? name = null, string? message = null) =>
        Error.NotFound(
            "entity.not.found",
            message ?? $"{name ?? "Entity"} was not found in the database.");

    public static Error CollectionIsNullOrEmpty(string? name = null) =>
        Error.Validation(
            "collection.is.empty",
            $"Collection {name ?? ""} cannot be null or empty",
            $"Collection {name ?? ""}");

    public static Error CollectionContainsDuplicates(string? name = null) =>
        Error.Validation(
            "collection.duplicates",
            $"Collection {name ?? ""} cannot contains duplicates",
            $"Collection {name ?? ""}");

    public static Error DatabaseReadFailed(string message, string? code = null) =>
        Error.Failure(code ?? "database.read.failed", message);

    public static Error DatabaseAddFailed(string message, string? code = null) =>
        Error.Failure(code ?? "database.add.failed", message);

    public static Error DatabaseUpdateFailed(string message, string? code = null) =>
        Error.Failure(code ?? "database.update.failed", message);

    public static Error DatabaseDeleteFailed(string message, string? code = null) =>
        Error.Failure(code ?? "database.delete.failed", message);

    public static Error DatabaseLockFailed(string message, string? code = null) =>
    Error.Failure(code ?? "database.lock.failed", message);

    public static Error OperationCancelled() =>
        Error.Failure("database.operation.cancelled", "Operation was cancelled");
}