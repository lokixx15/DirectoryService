using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Text.Json;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public class GetLocationsHandler
    : IQueryHandler<Result<LocationsResponse, Errors>, GetLocationsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetLocationsQuery> _validator;
    private readonly ILogger<GetLocationsHandler> _logger;


    public GetLocationsHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetLocationsQuery> validator,
        ILogger<GetLocationsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<LocationsResponse, Errors>> Handle(GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var queryValidationResult = await _validator.ValidateAsync(query);

        if (!queryValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating GetLocationsQuery");
            return queryValidationResult.ToErrors();
        }
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();
        var whereConditions = new List<string>();
        string joinCondition = string.Empty;

        if (query.Request.DepartmentIds != null)
        {
            parameters.Add("department_ids", query.Request.DepartmentIds);
            whereConditions.Add("d.id = ANY(@department_ids)");
            joinCondition =
                """
                JOIN department_location AS dl ON dl.location_id = l.id
                JOIN departments AS d ON dl.department_id = d.id
                """;
        }

        if (query.Request.Search !=  null)
        {
            parameters.Add("search", query.Request.Search);
            whereConditions.Add("l.name ILIKE '%' || @search || '%'");
        }

        if (query.Request.IsActive != null)
        {
            parameters.Add("is_active", query.Request.IsActive);
            whereConditions.Add("is_active = @is_active");
        }

        parameters.Add("page_size", query.Request.PageSize);
        parameters.Add("offset", (query.Request.Page - 1) * query.Request.PageSize);

        var orderBy = query.Request.OrderBy switch 
        {
            "name" => "l.name",
            "createdDate" => "l.created_at",
            "updatedDate" => "l.updated_at",
            _ => "l.created_at"
        };
        var orderDirection = query.Request.OrderDirection.ToUpper() == "ASC" ? "ASC" : "DESC";

        var fromClause = $"FROM locations AS l";
        var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";
        var orderByClause = $"ORDER BY {orderBy} {orderDirection}";            

        long? totalCount = null!;

        try
        {
            var locationResponseList = await connection.QueryAsync<LocationDto, string, long, LocationDto>(

                $"""
                    SELECT l.id,
                           l.name,
                           l.timezone,
                           l.created_at,
                           l.updated_at,
                           l.address,
                           COUNT(*) OVER() AS total_count
                    FROM locations AS l
                    {joinCondition}
                    {whereClause}
                    {orderByClause}
                    LIMIT @page_size OFFSET @offset
                 """,
                map: (lD, s, l) =>
                {
                    var address = JsonSerializer.Deserialize<LocationAddressDto>(s);

                    totalCount ??= l;

                    return lD with { Address = address! };
                },
                parameters,
                splitOn: "address,total_count");

            _logger.LogInformation("Locations have been received");

            return new LocationsResponse(locationResponseList.ToList(), totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when getting locations");
            return GeneralErrors.DatabaseReadFailed(ex.Message, "database.read.failed").ToErrors();
        }
    }
}