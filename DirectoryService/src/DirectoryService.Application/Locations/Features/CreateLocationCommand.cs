using DirectoryService.Contracts.Locations;
using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Locations.Features;

public record CreateLocationCommand(CreateLocationDto createLocationDto) : ICommand;
