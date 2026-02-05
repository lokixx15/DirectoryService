using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Contracts.Departments;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetDepartmentsWithMostPositions;

public class GetDepartmentsWithMostPositionsHandler 
    : IQueryHandler<Result<IReadOnlyList<DepartmentDto>, Errors>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<GetDepartmentsWithMostPositionsHandler> _logger;

    public GetDepartmentsWithMostPositionsHandler(
        IDbConnectionFactory connectionFactory,
        ILogger<GetDepartmentsWithMostPositionsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>, Errors>> Handle(
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.GetDbConnection();

        try
        {
            var locations = await connection.QueryAsync<DepartmentDto>(
            """
            SELECT d.id,
                   d.name,
                   d.identifier,
                   d.parent_id,
                   d.path,
                   d.depth,
                   d.is_active,
                   d.created_at,
                   d.updated_at,
                   COUNT(dp.id) AS positions_count
            FROM departments AS d
            JOIN department_position AS dp ON d.id = dp.department_id
            GROUP BY d.id
            ORDER BY positions_count DESC
            LIMIT 5;
            """, cancellationToken);

            _logger.LogInformation("Departments with most positions have been received");

            return locations.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when getting departments");
            return GeneralErrors.DatabaseReadFailed(ex.Message, "database.read.failed").ToErrors();
        }
    }
}