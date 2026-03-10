using DirectoryService.Contracts.Locations;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Locations.Features.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand;