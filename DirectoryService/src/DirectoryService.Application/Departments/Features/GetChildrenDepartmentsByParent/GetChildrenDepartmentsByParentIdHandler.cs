using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByParent;

public class GetChildrenDepartmentsByParentIdHandler
    : IQueryHandler<Result<PaginationResponse<DepartmentDto>, Errors>, GetChildrenDepartmentsByParentIdQuery>
{
    private const string sql = """
             WITH children AS (
             				   SELECT d.id,
             				   	      d.name,
             				   	      d.identifier,
             				   	      d.parent_id,
             				   	      d.path,      
             				   	      d.depth,
             				   	      d.is_active,
             				   	      d.created_at,
             				   	      d.updated_at,
                                      COUNT(*) OVER() AS total_count
             				   FROM departments AS d 
             				   WHERE d.parent_id = @parent_id
             				   LIMIT @children_limit OFFSET @offset
             )
             SELECT *, (EXISTS(SELECT 1 FROM departments WHERE parent_id = children.id))
             FROM children;
             """;

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetChildrenDepartmentsByParentIdQuery> _validator;
    private readonly HybridCache _cache;
    private readonly ILogger<GetChildrenDepartmentsByParentIdHandler> _logger;

    public GetChildrenDepartmentsByParentIdHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetChildrenDepartmentsByParentIdQuery> validator,
        HybridCache cache,
        ILogger<GetChildrenDepartmentsByParentIdHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<PaginationResponse<DepartmentDto>, Errors>> Handle(
        GetChildrenDepartmentsByParentIdQuery query,
        CancellationToken cancellationToken)
    {
        var queryValidationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!queryValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating GetChildrenDepartmentsByParentIdQuery");
            return queryValidationResult.ToErrors();
        }

        using var connection = _connectionFactory.GetDbConnection();

        var parameters = new DynamicParameters();

        parameters.Add("parent_id", query.ParentId);
        parameters.Add("children_limit", query.Request.Size);
        parameters.Add("offset", (query.Request.Page - 1) * query.Request.Size);

        long? totalCount = null!;

        var key = $"{CacheConstants.CHILDREN_DEPARTMENTS_CACHE_KEY}_parentid_{query.ParentId}_page_{query.Request.Page}_pagesize_{query.Request.Size}";

        var departmentDtos = await _cache.GetOrCreateAsync(
            key,
            async _ =>
            {
                using var connection = _connectionFactory.GetDbConnection();

                var departmentDtos = await connection.QueryAsync<DepartmentDto, long, DepartmentDto>(
                    sql,
                    map: (dD, l) =>
                    {
                        totalCount ??= l;

                        return dD;
                    },
                    parameters,
                    splitOn: "total_count");

                return departmentDtos.ToList();
            },
            tags:[CacheConstants.DEPARTMENTS_CACHE_TAG],
            cancellationToken: cancellationToken);

        return new PaginationResponse<DepartmentDto>(departmentDtos.ToList(), totalCount ?? 0);
    }
}