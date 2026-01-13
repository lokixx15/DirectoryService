using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Position with name {Name} has been added to the database", position.Name);

            return position.Id;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
                return GeneralErrors.ValueAlreadyExists("Position already exists");

            _logger.LogError(ex, "Database update error when creating position with name {Name}", position.Name.Value);

            return GeneralErrors.DatabaseInsertFailed(pgEx.Message);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled when creating position with name {Name}", position.Name.Value);
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when creating position with name {Name}", position.Name.Value);
            return GeneralErrors.DatabaseInsertFailed(ex.Message);
        }
    }
}
