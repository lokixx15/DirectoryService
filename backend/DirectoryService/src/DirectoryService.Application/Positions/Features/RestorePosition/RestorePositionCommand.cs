using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Positions.Features.RestorePosition;

public record RestorePositionCommand(Guid Id) : ICommand;
