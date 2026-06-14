using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions.Database;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Positions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Positions.Features.GetPositions;

public sealed class GetPositionsHandler
    : IQueryHandler<Result<CursorPaginationResponse<PositionDto>, Errors>, GetPositionsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetPositionsQuery> _validator;
    private readonly ILogger<GetPositionsHandler> _logger;

    public GetPositionsHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetPositionsQuery> validator,
        ILogger<GetPositionsHandler> logger)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<CursorPaginationResponse<PositionDto>, Errors>> Handle(
        GetPositionsQuery query,
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

        if (query.Request.Cursor != null)
        {
            var cursor = Cursor.Decode(query.Request.Cursor);
            if (cursor != null)
            {
                parameters.Add("id", cursor.Id);
                parameters.Add("created_at", cursor.CreatedAt);

                whereConditions.Add("""
                                    (p.created_at, p.id) < (@created_at, @id)
                                    """);
            }

        }

        if (query.Request.DepartmentIds != null)
        {
            parameters.Add("department_ids", query.Request.DepartmentIds);
            whereConditions.Add("""
                                EXISTS (
                                    SELECT 1
                                    FROM department_position AS dp
                                    WHERE dp.position_id = p.id 
                                        AND dp.department_id = ANY(@department_ids))
                                """);

        }

        if (!string.IsNullOrEmpty(query.Request.Search))
        {
            parameters.Add("search", query.Request.Search);
            whereConditions.Add("p.name ILIKE '%' || @search || '%'");
        }

        if (query.Request.IsActive != null)
        {
            parameters.Add("is_active", query.Request.IsActive);
            whereConditions.Add("p.is_active = @is_active");
        }

        parameters.Add("page_size", query.Request.PageSize + 1);

        var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : string.Empty;

        var postionDtos = await connection.QueryAsync<PositionDto>(
            $"""
                SELECT p.id,
                       p.name,
                       p.description,
                       p.is_active,
                       p.created_at,
                       p.updated_at,
                       p.deleted_at
                FROM positions p
                {whereClause}
                ORDER BY p.created_at DESC, p.id DESC
                LIMIT @page_size
             """,
            param: parameters);

        var positions = postionDtos.ToList();

        var hasNextPage = positions.Count > query.Request.PageSize;

        var items = hasNextPage
            ? positions.Take(query.Request.PageSize).ToList()
            : positions;

        var nextCursor = hasNextPage
            ? Cursor.Encode(
                positions[positions.Count - 1].Id,
                positions[positions.Count - 1].CreatedAt)
            : null;

        return new CursorPaginationResponse<PositionDto>(items, nextCursor);
    }
}
