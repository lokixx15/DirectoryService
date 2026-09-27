using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;

public sealed class GetRootDepartmentsWithChildrenHandler
    : IQueryHandler<Result<PaginationResponse<DepartmentDto>, Errors>, GetRootDepartmentsWithChildrenQuery>
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
                 					   d.updated_at,
                                       d.deleted_at,
                                       COUNT(*) FILTER (WHERE d.depth = 0) OVER() AS total_root_count
                 			    FROM departments AS d
                 				{rootsWhereClause}
                                ORDER BY d.name
                 				LIMIT @root_limit OFFSET @offset
                 )
                 SELECT r.*, (EXISTS(SELECT 1 FROM departments WHERE parent_id = r.id OFFSET @children_limit)) AS has_more_children
                 FROM roots AS r

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
                 					        d.updated_at,
                                            d.deleted_at,
                                            0 AS total_root_count	    
                 					 FROM departments AS d
                 					 {childrenWhereClause}
                                     ORDER BY d.name
                 					 LIMIT @children_limit) AS c;
                 """;

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetRootDepartmentsWithChildrenQuery> _validator;
    private readonly HybridCache _cache;
    private readonly ILogger<GetRootDepartmentsWithChildrenHandler> _logger;

    private sealed record CachedDepartmentsPage(List<DepartmentDto> Items, long TotalCount);

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

    public async Task<Result<PaginationResponse<DepartmentDto>, Errors>> Handle(
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

        var rootsWhereConditions = new List<string>() { "d.parent_id IS NULL" };

        if (query.Request.DepartmentIds != null && query.Request.DepartmentIds.Any())
        {
            parameters.Add("department_ids", query.Request.DepartmentIds);
            rootsWhereConditions.Add("d.id = ANY(@department_ids)");
        }

        if (query.Request.ExcludedDepartmentIds != null && query.Request.ExcludedDepartmentIds.Any())
        {
            parameters.Add("excluded_department_ids", query.Request.ExcludedDepartmentIds);
            rootsWhereConditions.Add("NOT (d.id = ANY(@excluded_department_ids))");
        }

        var childrenWhereConditions = new List<string>() { "r.id = d.parent_id" };

        if (query.Request.IsActiveOnly != false)
        {
            parameters.Add("is_root_active", query.Request.IsActiveOnly);
            rootsWhereConditions.Add("d.is_active = @is_root_active");

            parameters.Add("is_child_active", query.Request.IsActiveOnly);
            childrenWhereConditions.Add("d.is_active = @is_child_active");
        }

        var rootsWhereClause = rootsWhereConditions.Any() ? "WHERE " + string.Join(" AND ", rootsWhereConditions) : string.Empty;
        var childrenWhereClause = childrenWhereConditions.Any() ? "WHERE " + string.Join(" AND ", childrenWhereConditions) : string.Empty;

        var deptIdsKey = query.Request.DepartmentIds != null ? string.Join(",", query.Request.DepartmentIds) : "all";
        var exclIdsKey = query.Request.ExcludedDepartmentIds != null ? string.Join(",", query.Request.ExcludedDepartmentIds) : "none";

        var key = $"{CacheConstants.ROOT_DEPARTMENTS_WITH_CHILDREN_CACHE_KEY}_page_{query.Request.Page}_size_{query.Request.Size}_prefetch_{query.Request.Prefetch}_ids_{deptIdsKey}_excl_{exclIdsKey}_isactiveonly_{query.Request.IsActiveOnly}";

        var finalSql = SQL
            .Replace("{rootsWhereClause}", rootsWhereClause)
            .Replace("{childrenWhereClause}", childrenWhereClause);

        var cachedData = await _cache.GetOrCreateAsync(
            key,
            async _ =>
            {
                using var connection = _connectionFactory.GetDbConnection();

                long? totalCount = null!;

                var departmentDtos = await connection.QueryAsync<DepartmentDto, long, bool, DepartmentDto>(
                    finalSql,
                    map: (dD, l, b) =>
                    {
                        totalCount ??= l;
                        dD.HasMoreChildren = b;

                        return dD;
                    },
                    parameters,
                    splitOn: "total_root_count,has_more_children");

                return new CachedDepartmentsPage(departmentDtos.ToList(), totalCount ?? 0);
            },
            tags: [CacheConstants.DEPARTMENTS_CACHE_TAG],
            cancellationToken: cancellationToken);

        return new PaginationResponse<DepartmentDto>(cachedData.Items, cachedData.TotalCount);
    }
}
