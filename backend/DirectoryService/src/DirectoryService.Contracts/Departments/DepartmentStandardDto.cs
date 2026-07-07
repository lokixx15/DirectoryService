namespace DirectoryService.Contracts.Departments;

public record DepartmentStandardDto(
    Guid Id,
    string Name,
    string Identifier,
    string Path,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DeletedAt);
