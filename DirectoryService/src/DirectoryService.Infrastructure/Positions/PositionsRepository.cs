using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions;
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

        _logger.LogInformation("Position was addedd to the database");

        return position.Id;
    }

    public async Task<UnitResult<Error>> SoftDeletePositionsWithoutActiveDepartments(Guid departmentId, CancellationToken cancellationToken)
    {
        var sql = @"
                    WITH department_positions AS (
                    							  SELECT p.*
                    							  FROM positions AS p
                    							  JOIN department_position AS dp ON dp.position_id = p.id
                    							  WHERE dp.department_id = @departmentId)								  
                    UPDATE positions 
                    SET is_active = false,
                    	deleted_at = NOW() AT TIME ZONE 'UTC'
                    WHERE id IN (
                    	         SELECT dps.id
                    	         FROM department_positions AS dps
                    	         WHERE NOT EXISTS (
                    	         				   SELECT 1
                    	                           FROM department_position AS dp
                    	                           JOIN departments AS d ON dp.department_id = d.id
                    	                           WHERE dp.department_id != @departmentId
                    	         					 AND dp.position_id = dps.id
                    	         	                 AND d.is_active = true) 
                          AND dps.is_active = true);
                    ";           

        await _dbContext.Database.ExecuteSqlRawAsync(
            sql,
            [new NpgsqlParameter("@departmentId", departmentId)],
            cancellationToken);

        return UnitResult.Success<Error>();
    }
}