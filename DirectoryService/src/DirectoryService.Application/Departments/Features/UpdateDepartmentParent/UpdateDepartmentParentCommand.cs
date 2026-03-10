using DirectoryService.Contracts.Departments;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentParent;

public record UpdateDepartmentParentCommand(
    Guid departmentId,
    UpdateDepartmentParentRequest Request) : ICommand;