using System.Text.Json.Serialization;

namespace SharedKernel;

public class Error
{
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

    public static Error None() => new("", "", ErrorType.NONE);

    public static Error Validation(string? code, string message, string? invalidField) =>
        new(code ?? "value.is.not.valid", message, ErrorType.VALIDATION, invalidField);

    public static Error NotFound(string? code, string message) =>
        new(code ?? "value.is.not.found", message, ErrorType.NOT_FOUND);

    public static Error Failure(string? code, string message) =>
        new(code ?? "failure", message, ErrorType.FAILURE);

    public static Error Conflict(string? code, string message, string? invalidField) =>
        new(code ?? "value.is.conflict", message, ErrorType.CONFLICT);

    public Errors ToErrors() => this;
}

public enum ErrorType
{
    NONE,
    VALIDATION,
    NOT_FOUND,
    FAILURE,
    CONFLICT
}
