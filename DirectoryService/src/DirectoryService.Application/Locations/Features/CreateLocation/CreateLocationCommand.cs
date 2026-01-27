using DirectoryService.Contracts.Locations;
using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Locations.Features.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand;