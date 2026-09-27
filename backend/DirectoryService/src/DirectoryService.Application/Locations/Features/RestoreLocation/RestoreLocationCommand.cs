using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.RestoreLocation;

public record RestoreLocationCommand(Guid Id) : ICommand;
