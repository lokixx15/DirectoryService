using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Departments.Features.RestoreDepartment;

public record RestoreDepartmentCommand(Guid Id) : ICommand;
