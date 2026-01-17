using CSharpFunctionalExtensions;
using SharedKernel;

namespace DirectoryService.Domain.Departments.VO;

public record DepartmentPath
{
    //ef core 
    private DepartmentPath() { }

    public const string SEPARATOR = ".";
    private DepartmentPath(
        string value)
    {
        Value = value;
    }

    public string Value { get; } = string.Empty;

    public static Result<DepartmentPath, Errors> Create(string value, string? parentPath = null)
    {
        var errors = new List<Error>();

        if (value.Length > Constants.MAX_DEPARTMENT_PATH_LENGTH)
            errors.Add(GeneralErrors.ValueLengthIsNotValid(Constants.MAX_DEPARTMENT_PATH_LENGTH, "Path"));

        if (errors.Any())
            return Result.Failure<DepartmentPath, Errors>(errors);

        var pathPrefix = parentPath != null ? $"{parentPath}{SEPARATOR}" : "";

        var path = new DepartmentPath($"{pathPrefix}{value}");

        return Result.Success<DepartmentPath, Errors>(path);
    }
}