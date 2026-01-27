using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Departments.Features.UpdateDepartmentLocations;

public record UpdateDepartmentLocationsCommand(
    Guid departmentId,
    UpdateDepartmentLocationsRequest Request) : ICommand;
