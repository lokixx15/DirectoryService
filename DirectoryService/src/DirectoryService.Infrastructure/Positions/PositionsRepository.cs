using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Infrastructure.Positions;

public class PositionsRepository : IPositionsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<PositionsRepository> _logger;

    public PositionsRepository(
        DirectoryServiceDbContext dbContext, 
        ILogger<PositionsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(position, cancellationToken);

        _logger.LogInformation("Position was addedd to the database");

        return position.Id;
    }
}