using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments;

namespace DirectoryService.Application.Departments.Features.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : ICommand;