using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.Database;
using FileService.Core.Abstractions.FileStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Framework.Endpoints;
using SharedService.SharedKernel;

namespace FileService.Core.Features;

public record GetDownloadUrlQuery(Guid MediaAssetId) : IQuery;

public class GetDownloadUrlEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{mediaAssetId:guid}/url", async Task<EndpointResult<string>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] GetDownloadUrlHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new GetDownloadUrlQuery(mediaAssetId), cancellationToken));
    }
}

public class GetDownloadUrlHandler : IQueryHandler<Result<string, Errors>, GetDownloadUrlQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IReadFileServiceDbContext _readDbContext;
    private readonly ILogger<GetDownloadUrlHandler> _logger;

    public GetDownloadUrlHandler(
        IS3Provider s3Provider,
        IReadFileServiceDbContext readDbContext,
        ILogger<GetDownloadUrlHandler> logger)
    {
        _s3Provider = s3Provider;
        _readDbContext = readDbContext;
        _logger = logger;
    }

    public async Task<Result<string, Errors>> Handle(
        GetDownloadUrlQuery query,
        CancellationToken cancellationToken)
    {
        if (query.MediaAssetId == Guid.Empty)
            return GeneralErrors.ValueIsNotValid("Guid cannot be empty", nameof(query.MediaAssetId)).ToErrors();

        var mediaAssetResult = await _readDbContext.ReadMediaAssets
            .FirstOrDefaultAsync(ma => ma.Id == query.MediaAssetId, cancellationToken);

        if (mediaAssetResult == null)
            return GeneralErrors.EntityNotFound("Media asset").ToErrors();

        var downloadResult = await _s3Provider.GenerateDownloadUrlAsync(mediaAssetResult.FinalKey);
        if (downloadResult.IsFailure)
        {
            _logger.LogError("Errors occurred when getting download url of file with id {Id}", query.MediaAssetId);
            return downloadResult.Error.ToErrors();
        }

        return downloadResult.Value;
    }
}