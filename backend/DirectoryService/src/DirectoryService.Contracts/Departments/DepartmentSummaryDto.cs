namespace DirectoryService.Contracts.Departments;

public record DepartmentSummaryDto(
    Guid Id,
    string Name,
    string Identifier);