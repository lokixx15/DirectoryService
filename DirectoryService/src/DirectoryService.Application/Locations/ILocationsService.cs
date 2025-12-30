using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using SharedKernel;

namespace DirectoryService.Application.Locations;

public interface ILocationsService
{
    Task<Result<Guid, Errors>> Create(CreateLocationDto locationDto, CancellationToken cancellationToken);
}
