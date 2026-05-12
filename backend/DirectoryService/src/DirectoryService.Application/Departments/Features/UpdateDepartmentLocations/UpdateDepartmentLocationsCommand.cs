using DirectoryService.Contracts.Departments;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;

public record UpdateDepartmentLocationsCommand(
    Guid departmentId,
    UpdateDepartmentLocationsRequest Request) : ICommand;