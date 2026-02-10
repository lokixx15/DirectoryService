using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Departments.Features.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(Guid departmentId)
    : ICommand;
