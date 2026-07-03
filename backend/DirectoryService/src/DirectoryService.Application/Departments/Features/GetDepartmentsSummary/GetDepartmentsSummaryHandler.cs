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

public sealed class GetDepartmentsSummaryHandler : IQueryHandler<Result<PaginationResponse<DepartmentSummaryDto>, Errors>, GetDepartmentsSummaryQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;

    private readonly IValidator<GetDepartmentsSummaryQuery> _validator;
    private readonly ILogger<GetDepartmentsSummaryHandler> _logger;

    public GetDepartmentsSummaryHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetDepartmentsSummaryQuery> validator,
        ILogger<GetDepartmentsSummaryHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PaginationResponse<DepartmentSummaryDto>, Errors>> Handle(
        GetDepartmentsSummaryQuery query,
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

        string whereCondition = string.Empty;

        if (!string.IsNullOrEmpty(query.Request.Search))
        {
            parameters.Add("search", query.Request.Search);
            whereCondition = "WHERE name ILIKE '%' || @search || '%'";
        }

        long? totalCount = null;

        var departmentDtos = await connection.QueryAsync<DepartmentSummaryDto, long, DepartmentSummaryDto>(
            $"""
                SELECT id,
                       name,
                       identifier,
                       COUNT(*) OVER() AS total_count
                FROM departments
                {whereCondition}
                LIMIT @page_size OFFSET @offset
             """,
            map: (dSD, l) =>
            {
                totalCount ??= l;
                return dSD;
            },
            parameters,
            splitOn: "total_count");

        return new PaginationResponse<DepartmentSummaryDto>(departmentDtos.ToList(), totalCount ?? 0);
    }
}
