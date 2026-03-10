using System.Text.Json.Serialization;

namespace SharedService.SharedKernel;

public sealed class Error
{
    public const string SEPARATOR = "||";

    public string Code { get; } = string.Empty;

    public string Message { get; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }

    public string? InvalidField { get; }

    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error None() => new(string.Empty, string.Empty, ErrorType.NONE);

    public static Error Validation(string? code, string message, string? invalidField) =>
        new(code ?? "value.is.not.valid", message, ErrorType.VALIDATION, invalidField);

    public static Error NotFound(string? code, string message) =>
        new(code ?? "value.is.not.found", message, ErrorType.NOT_FOUND);

    public static Error Failure(string? code, string message) =>
        new(code ?? "failure", message, ErrorType.FAILURE);

    public static Error Conflict(string? code, string message) =>
        new(code ?? "value.is.conflict", message, ErrorType.CONFLICT);

    public static Error Deserialize(string error)
    {
        string[] errorParts = error.Split(SEPARATOR);

        if (errorParts.Length < 3)
            throw new ArgumentException("Error string has an invalid format for deserialization");

        if (Enum.TryParse<ErrorType>(errorParts[2], out var type) == false)
            throw new ArgumentException("Error string has an invalid format for deserialization");

        return new(errorParts[0], errorParts[1], type);
    }

    public Errors ToErrors() => this;

    public string GetMessage() => Message;

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type, InvalidField);
    }
}

public enum ErrorType
{
    NONE,
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT,
}