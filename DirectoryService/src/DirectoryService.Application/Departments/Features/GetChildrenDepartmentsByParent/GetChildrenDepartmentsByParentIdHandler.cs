using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetChildrenDepartmentsByRootId;

public class GetChildrenDepartmentsByParentIdHandler
    : IQueryHandler<Result<PaginationResponse<DepartmentDto>, Errors>, GetChildrenDepartmentsByParentIdQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetChildrenDepartmentsByParentIdQuery> _validator;
    private readonly ILogger<GetChildrenDepartmentsByParentIdHandler> _logger;

    public GetChildrenDepartmentsByParentIdHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetChildrenDepartmentsByParentIdQuery> validator,
        ILogger<GetChildrenDepartmentsByParentIdHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
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

        var locationResponseList = await connection.QueryAsync<DepartmentDto, long, DepartmentDto>(
            $"""
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
             """,
            map: (dD, l) =>
            {
                totalCount ??= l;

                return dD;
            },
            parameters,
            splitOn: "total_count");

        return new PaginationResponse<DepartmentDto>(locationResponseList.ToList(), totalCount ?? 0);
    }
}