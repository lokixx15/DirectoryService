using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Application.Caching;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public sealed class GetLocationsHandler : IQueryHandler<Result<PaginationResponse<LocationDto>, Errors>, GetLocationsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetLocationsQuery> _validator;
    private readonly HybridCache _cache;
    private readonly ILogger<GetLocationsHandler> _logger;

    public GetLocationsHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetLocationsQuery> validator,
        HybridCache cache,
        ILogger<GetLocationsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<PaginationResponse<LocationDto>, Errors>> Handle(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var queryValidationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!queryValidationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating GetLocationsQuery");
            return queryValidationResult.ToErrors();
        }

        using var connection = _connectionFactory.GetDbConnection();

        var parameters = new DynamicParameters();
        var whereConditions = new List<string>();

        if (query.Request.SelectedDepartmentIds != null)
        {
            parameters.Add("department_ids", query.Request.SelectedDepartmentIds);
            whereConditions.Add("""
                                EXISTS(
                                    SELECT 1
                                    FROM department_location AS dl
                                    WHERE dl.location_id = l.id AND 
                                    dl.department_id = ANY(@department_ids))
                                """);
        }

        if (query.Request.ExcludedDepartmentIds != null)
        {
            parameters.Add("excluded_department_ids", query.Request.ExcludedDepartmentIds);
            whereConditions.Add("""
                        NOT EXISTS(
                            SELECT 1
                            FROM department_location AS dl
                            WHERE dl.location_id = l.id AND 
                            dl.department_id = ANY(@excluded_department_ids))
                        """);
        }

        if (!string.IsNullOrEmpty(query.Request.Search))
        {
            parameters.Add("search", query.Request.Search);
            whereConditions.Add("l.name ILIKE '%' || @search || '%'");
        }

        if (query.Request.IsActive != null)
        {
            parameters.Add("is_active", query.Request.IsActive);
            whereConditions.Add("l.is_active = @is_active");
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

        var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : string.Empty;
        var orderByClause = $"ORDER BY {orderBy} {orderDirection}";

        var key = $"{CacheConstants.LOCATIONS}" +
          $"_page_{query.Request.Page}" +
          $"_size_{query.Request.PageSize}" +
          $"_search_{query.Request.Search}" +
          $"_active_{query.Request.IsActive}" +
          $"_ids_{query.Request.SelectedDepartmentIds}" +
          $"_excl_{query.Request.ExcludedDepartmentIds}" +
          $"_order_{query.Request.OrderBy}" +
          $"_dir_{query.Request.OrderDirection}";
        long? totalCount = null!;

        var locationDtos = await _cache.GetOrCreateAsync(
            key,
            async _ =>
            {
                return await connection.QueryAsync<LocationDto, string, long, LocationDto>(
                    $"""
                SELECT l.id,
                       l.name,
                       l.timezone,
                       l.created_at,
                       l.updated_at,
                       l.deleted_at,
                       l.address,
                       COUNT(*) OVER() AS total_count
                FROM locations AS l
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
            },
            tags: [CacheConstants.LOCATIONS_CACHE_TAG],
            cancellationToken: cancellationToken);

        return new PaginationResponse<LocationDto>(locationDtos.ToList(), totalCount ?? 0);
    }
}