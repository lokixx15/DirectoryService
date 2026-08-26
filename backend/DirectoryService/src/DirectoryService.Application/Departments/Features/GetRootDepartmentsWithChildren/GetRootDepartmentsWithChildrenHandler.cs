using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using DirectoryService.Contracts.Departments;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;

public sealed class GetRootDepartmentsWithChildrenHandler
    : IQueryHandler<Result<IReadOnlyList<DepartmentDto>, Errors>, GetRootDepartmentsWithChildrenQuery>
{
    private const string SQL = """
                 WITH roots AS (
                 				SELECT d.id,
                 					   d.name,
                 					   d.identifier,
                 					   d.parent_id,
                 					   d.path,
                 					   d.depth,
                 					   d.is_active,
                 					   d.created_at,
                 					   d.updated_at
                 			    FROM departments AS d
                 				{whereClause}
                 				LIMIT @root_limit OFFSET @offset
                 )
                 SELECT *, (EXISTS(SELECT 1 FROM departments WHERE parent_id = roots.id OFFSET @children_limit)) AS has_more_children
                 FROM roots

                 UNION ALL 

                 SELECT c.*, (EXISTS(SELECT 1 FROM departments WHERE parent_id = c.id)) AS has_more_children
                 FROM roots AS r
                 CROSS JOIN LATERAL (
                 					 SELECT d.id,
                 					 	    d.name,
                 					    	d.identifier,
                 					    	d.parent_id,
                 					    	d.path,
                 					    	d.depth,
                 					    	d.is_active,
                 					 	    d.created_at,
                 					        d.updated_at	    
                 					 FROM departments AS d
                 					 WHERE r.id = d.parent_id
                 					 LIMIT @children_limit) AS c;
                 """;

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetRootDepartmentsWithChildrenQuery> _validator;
    private readonly HybridCache _cache;
    private readonly ILogger<GetRootDepartmentsWithChildrenHandler> _logger;

    public GetRootDepartmentsWithChildrenHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetRootDepartmentsWithChildrenQuery> validator,
        HybridCache cache,
        ILogger<GetRootDepartmentsWithChildrenHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>, Errors>> Handle(
        GetRootDepartmentsWithChildrenQuery query,
        CancellationToken cancellationToken)
    {
        var queryValidationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!queryValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating GetRootDepartmentsWithChildrenQuery");
            return queryValidationResult.ToErrors();
        }

        var parameters = new DynamicParameters();

        parameters.Add("root_limit", query.Request.Size);
        parameters.Add("offset", (query.Request.Page - 1) * query.Request.Size);
        parameters.Add("children_limit", query.Request.Prefetch);

        var whereConditions = new List<string>() { "d.parent_id IS NULL", "d.is_active = true" };

        if (query.Request.DepartmentIds != null && query.Request.DepartmentIds.Any())
        {
            parameters.Add("department_ids", query.Request.DepartmentIds);
            whereConditions.Add("d.id = ANY(@department_ids)");
        }

        if (query.Request.ExcludedDepartmentIds != null && query.Request.ExcludedDepartmentIds.Any())
        {
            parameters.Add("excluded_department_ids", query.Request.ExcludedDepartmentIds);
            whereConditions.Add("NOT (d.id = ANY(@excluded_department_ids))");
        }

        var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : string.Empty;

        var deptIdsKey = query.Request.DepartmentIds != null ? string.Join(",", query.Request.DepartmentIds) : "all";
        var exclIdsKey = query.Request.ExcludedDepartmentIds != null ? string.Join(",", query.Request.ExcludedDepartmentIds) : "none";

        var key = $"{CacheConstants.ROOT_DEPARTMENTS_WITH_CHILDREN_CACHE_KEY}_page_{query.Request.Page}_size_{query.Request.Size}_prefetch_{query.Request.Prefetch}_ids_{deptIdsKey}_excl_{exclIdsKey}";

        var finalSql = SQL.Replace("{whereClause}", whereClause);

        var departmentDtos = await _cache.GetOrCreateAsync(
            key,
            async _ =>
            {
                using var connection = _connectionFactory.GetDbConnection();

                var departmentDtos = await connection.QueryAsync<DepartmentDto>(
                    finalSql,
                    parameters);

                return departmentDtos.ToList();
            },
            tags: [CacheConstants.DEPARTMENTS_CACHE_TAG],
            cancellationToken: cancellationToken);

        return departmentDtos;
    }
}
