namespace DirectoryService.Contracts.Departments;

public record CreateDepartmentDto(
    string Name,
    string Identifier,
    Guid? ParentId,
    Guid[] LocationIds);