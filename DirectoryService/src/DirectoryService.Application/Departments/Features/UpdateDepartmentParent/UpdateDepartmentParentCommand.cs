using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentParent;

public record UpdateDepartmentParentCommand(
    Guid departmentId,
    Guid? parentId) : ICommand;