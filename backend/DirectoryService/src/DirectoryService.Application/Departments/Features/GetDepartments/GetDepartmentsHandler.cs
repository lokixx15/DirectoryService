using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Locations.Features.GetLocations;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.GetDepartments;

public sealed class GetDepartmentsHandler : IQueryHandler<Result<PaginationResponse<DepartmentStandardDto>, Errors>, GetDepartmentsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetDepartmentsQuery> _validator;
    private readonly ILogger<GetDepartmentsHandler> _logger;

    public GetDepartmentsHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetDepartmentsQuery> validator,
        ILogger<GetDepartmentsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PaginationResponse<DepartmentStandardDto>, Errors>> Handle(
        GetDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var queryValidationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!queryValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating GetDepartmentsQuery");
            return queryValidationResult.ToErrors();
        }

        using var connection = _connectionFactory.GetDbConnection();

        var parameters = new DynamicParameters();
        parameters.Add("page_size", query.Request.pageSize);
        parameters.Add("offset", (query.Request.Page - 1) * query.Request.pageSize);

        var whereConditions = new List<string>();

        if (!string.IsNullOrEmpty(query.Request.Search))
        {
            parameters.Add("search", query.Request.Search);
            whereConditions.Add("name ILIKE '%' || @search || '%'");
        }

        if (query.Request.ParentId != null)
        {
            parameters.Add("parentId", query.Request.ParentId);
            whereConditions.Add("parent_id = @parentId");
        }

        if (query.Request.LocationIds != null)
        {
            parameters.Add("locationIds", query.Request.LocationIds);
            whereConditions.Add("""
                                EXISTS(
                                    SELECT 1
                                    FROM department_location AS dl
                                    WHERE dl.department_id = d.id AND 
                                        dl.location_id = ANY(@locationIds))
                                """);
        }

        if (query.Request.ExcludeDepartmentIds != null)
        {
            parameters.Add("excludeIds", query.Request.ExcludeDepartmentIds);
            whereConditions.Add("d.id != ALL(@excludeIds)");
        }

        var orderBy = query.Request.OrderBy switch
        {
            "name" => "d.name",
            "createdDate" => "d.created_at",
            "updatedDate" => "d.updated_at",
            _ => "d.created_at"
        };

        var orderDirection = query.Request.OrderDirection.ToUpper() == "ASC" ? "ASC" : "DESC";

        var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : string.Empty;
        var orderByClause = $"ORDER BY {orderBy} {orderDirection}";

        long? totalCount = null;

        var departmentDtos = await connection.QueryAsync<DepartmentStandardDto, long, DepartmentStandardDto>(
            $"""
                SELECT d.id,
                       d.name,
                       d.identifier,
                       d.path,
                       d.is_active,
                       d.created_at,
                       d.updated_at,
                       d.deleted_at,
                       COUNT(*) OVER() AS total_count
                FROM departments AS d
                {whereClause}
                {orderByClause}
                LIMIT @page_size OFFSET @offset
             """,
            map: (dSD, l) =>
            {
                totalCount ??= l;
                return dSD;
            },
            parameters,
            splitOn: "total_count");

        return new PaginationResponse<DepartmentStandardDto>(departmentDtos.ToList(), totalCount ?? 0);
    }
}
