using CSharpFunctionalExtensions;
using SharedService.SharedKernel;

namespace DirectoryService.Domain.Departments.VO;

public record DepartmentPath
{
    // ef core
    private DepartmentPath() { }

    public const char SEPARATOR = '.';
    private DepartmentPath(
        string value)
    {
        Value = value;
    }

    public string Value { get; } = string.Empty;

    public static Result<DepartmentPath, Errors> Create(string value, string? parentPath = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(value))
            errors.Add(GeneralErrors.ValueIsNullOrWhitespace("Path"));

        if (value.Length > Constants.MAX_DEPARTMENT_PATH_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotValid(Constants.MAX_DEPARTMENT_PATH_LENGTH, "Path"));

        if (errors.Any())
            return Result.Failure<DepartmentPath, Errors>(errors);

        var pathPrefix = parentPath != null ? $"{parentPath}{SEPARATOR}" : string.Empty;

        var path = new DepartmentPath($"{pathPrefix}{value}");

        return Result.Success<DepartmentPath, Errors>(path);
    }

    public static Result<DepartmentPath, Errors> MarkAsDeleted(string path, string identifier)
    {
        var pathWithDeletedMark = path.Replace(identifier, "deleted-" + identifier);

        var newPathResult = Create(pathWithDeletedMark);

        if (newPathResult.IsFailure)
            return newPathResult.Error;

        return Result.Success<DepartmentPath, Errors>(newPathResult.Value);
    }
}