using DirectoryService.Contracts.Locations;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.UpdateLocation;

public record UpdateLocationCommand(Guid Id, UpdateLocationRequest Request) : ICommand;