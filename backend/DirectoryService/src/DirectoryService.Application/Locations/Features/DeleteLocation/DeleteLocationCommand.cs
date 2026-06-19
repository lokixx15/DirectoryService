using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.DeleteLocation;

public record DeleteLocationCommand(Guid Id) : ICommand;