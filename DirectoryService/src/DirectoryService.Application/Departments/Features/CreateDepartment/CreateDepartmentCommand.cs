using DirectoryService.Contracts.Departments;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : ICommand;