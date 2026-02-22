using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using DirectoryService.Contracts.Departments;
using Microsoft.Extensions.Caching.Hybrid;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetDepartmentsWithMostPositions;

public class GetDepartmentsWithMostPositionsHandler 
    : IQueryHandler<Result<IReadOnlyList<DepartmentDto>, Errors>>
{
    private const string sql = """
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
        """;

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly HybridCache _cache;

    public GetDepartmentsWithMostPositionsHandler(
        IDbConnectionFactory connectionFactory,
        HybridCache cache)
    {
        _connectionFactory = connectionFactory;
        _cache = cache;
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>, Errors>> Handle(
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.GetDbConnection();

        var key = $"{CacheConstants.DEPARTMENTS_WITH_MOST_POSITIONS_CACHE_KEY}";

        var departmentDtos = await _cache.GetOrCreateAsync(
            key,
            async _ =>
            {
                using var connection = _connectionFactory.GetDbConnection();

                var departmentDtos = await connection.QueryAsync<DepartmentDto>(
                    sql,
                    cancellationToken);

                return departmentDtos.ToList();
            },
            tags: [CacheConstants.DEPARTMENTS_CACHE_TAG],
            cancellationToken: cancellationToken);

        return departmentDtos;
    }
}