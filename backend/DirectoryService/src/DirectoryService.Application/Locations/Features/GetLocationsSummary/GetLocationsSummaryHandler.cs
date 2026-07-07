using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Locations.Features.GetLocations;

public sealed class GetLocationsSummaryHandler : IQueryHandler<Result<PaginationResponse<LocationSummaryDto>, Errors>, GetLocationsSummaryQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetLocationsSummaryQuery> _validator;
    private readonly ILogger<GetLocationsSummaryHandler> _logger;

    public GetLocationsSummaryHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetLocationsSummaryQuery> validator,
        ILogger<GetLocationsSummaryHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PaginationResponse<LocationSummaryDto>, Errors>> Handle(
        GetLocationsSummaryQuery query,
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
        parameters.Add("page_size", query.Request.pageSize);
        parameters.Add("offset", (query.Request.Page - 1) * query.Request.pageSize);

        string whereCondition = string.Empty;

        if (!string.IsNullOrEmpty(query.Request.Search))
        {
            parameters.Add("search", query.Request.Search);
            whereCondition = "WHERE name ILIKE '%' || @search || '%'";
        }

        long? totalCount = null;

        var locationDtos = await connection.QueryAsync<LocationSummaryDto, string, long, LocationSummaryDto>(
            $"""
                SELECT id,
                       name,
                       timezone,
                       address,
                       COUNT(*) OVER() AS total_count
                FROM locations
                {whereCondition}
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

        return new PaginationResponse<LocationSummaryDto>(locationDtos.ToList(), totalCount ?? 0);
    }
}
