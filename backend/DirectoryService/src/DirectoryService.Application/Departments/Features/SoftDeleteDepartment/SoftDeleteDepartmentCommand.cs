using DirectoryService.Application.Abstractions;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(Guid departmentId)
    : ICommand;