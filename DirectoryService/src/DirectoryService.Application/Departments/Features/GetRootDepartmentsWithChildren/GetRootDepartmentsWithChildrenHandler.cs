using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetRootDepartmentsWithChildren;

public sealed class GetRootDepartmentsWithChildrenHandler
    : IQueryHandler<Result<IReadOnlyList<DepartmentDto>, Errors>, GetRootDepartmentsWithChildrenQuery>
{
    private const string sql = """
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
                 				WHERE d.parent_id IS NULL AND d.is_active = true 
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

        parameters.Add("root_limit", query.Request.Pagination.Size);
        parameters.Add("offset", (query.Request.Pagination.Page - 1) * query.Request.Pagination.Size);
        parameters.Add("children_limit", query.Request.Prefetch);

        var key = $"{CacheConstants.ROOT_DEPARTMENTS_WITH_CHILDREN_CACHE_KEY}_page_{query.Request.Pagination.Page}_pagesize_{query.Request.Pagination.Size}_prefetch_{query.Request.Prefetch}";

        var departmentDtos = await _cache.GetOrCreateAsync(
            key,
            async _ =>
            {
                using var connection = _connectionFactory.GetDbConnection();

                var departmentDtos = await connection.QueryAsync<DepartmentDto>(
                    sql,
                    parameters);

                return departmentDtos.ToList();
            },
            tags: [CacheConstants.DEPARTMENTS_CACHE_TAG],
            cancellationToken: cancellationToken);

        return departmentDtos;
    }
}