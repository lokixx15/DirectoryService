namespace SharedKernel;

public class Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public string? InvalidField { get; }

    private Error(
        string code, 
        string message,
        ErrorType type, 
        string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error Validation(string? code, string message, string? invalidField = null) =>
        new Error(code ?? "value.is.not.valid", message, ErrorType.VALIDATION, invalidField);

    public static Error NotFound(string? code, string message) =>
    new Error(code ?? "value.is.not.found", message, ErrorType.NOT_FOUND);

    public static Error Failure(string? code, string message) =>
    new Error(code ?? "failure", message, ErrorType.FAILURE);

    public static Error Conflict(string? code, string message) =>
    new Error(code ?? "value.is.conflict", message, ErrorType.CONFLICT);
}
