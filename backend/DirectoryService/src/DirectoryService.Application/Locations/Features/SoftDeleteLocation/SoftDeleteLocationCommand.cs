using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.SoftDeleteLocation;

public record SoftDeleteLocationCommand(Guid Id) : ICommand;
