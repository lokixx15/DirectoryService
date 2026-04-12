using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.Database;
using FileService.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Framework.Endpoints;
using SharedService.SharedKernel;

namespace FileService.Core.Features;

public record CheckVideoExistenceQuery(Guid MediaAssetId) : IQuery;

public class CheckVideoExistenceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{mediaAssetId:guid}/exists", async Task<EndpointResult<CheckVideoExistenceResponse>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] CheckVideoExistenceHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new CheckVideoExistenceQuery(mediaAssetId), cancellationToken));
    }
}

public class CheckVideoExistenceHandler : IQueryHandler<Result<CheckVideoExistenceResponse, Error>, CheckVideoExistenceQuery>
{
    private readonly IReadFileServiceDbContext _readDbContext;
    private readonly ILogger<CheckVideoExistenceHandler> _logger;

    public CheckVideoExistenceHandler(
        IReadFileServiceDbContext readDbContext,
        ILogger<CheckVideoExistenceHandler> logger)
    {
        _readDbContext = readDbContext;
        _logger = logger;
    }

    public async Task<Result<CheckVideoExistenceResponse, Error>> Handle(CheckVideoExistenceQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await _readDbContext.ReadMediaAssets
                .AnyAsync(a => a.Id == query.MediaAssetId && a.AssetType == AssetType.VIDEO, cancellationToken);

            return new CheckVideoExistenceResponse(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking media asset existence for ID {MediaAssetId}", query.MediaAssetId);
            return Error.Failure("internal.error", "An error occurred while checking media asset existence.");
        }
    }
}