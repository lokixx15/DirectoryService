using DirectoryService.Contracts.Departments;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.AttachDepartmentVideo;

public record AttachDepartmentVideoCommand(
    Guid DepartmentId,
    AttachDepartmentVideoRequest Request)
    : ICommand;