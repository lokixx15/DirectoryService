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

public record DownloadFileQuery(Guid MediaAssetId) : IQuery;

public class DownloadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{mediaAssetId:guid}", async Task<EndpointResult<string>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] DownloadFileHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new DownloadFileQuery(mediaAssetId), cancellationToken));
    }
}

public class DownloadFileHandler : IQueryHandler<Result<string, Errors>, DownloadFileQuery>
{
    private const string PATH_TO_DOWNLOAD = @"C:\files";

    private readonly IS3Provider _s3Provider;
    private readonly IReadFileServiceDbContext _readDbContext;
    private readonly ILogger<DownloadFileHandler> _logger;

    public DownloadFileHandler(
        IS3Provider s3Provider,
        IReadFileServiceDbContext readDbContext,
        ILogger<DownloadFileHandler> logger)
    {
        _s3Provider = s3Provider;
        _readDbContext = readDbContext;
        _logger = logger;
    }

    public async Task<Result<string, Errors>> Handle(
        DownloadFileQuery query,
        CancellationToken cancellationToken)
    {
        if (query.MediaAssetId == Guid.Empty)
            return GeneralErrors.ValueIsNotValid("Guid cannot be empty", nameof(query.MediaAssetId)).ToErrors();

        var mediaAssetResult = await _readDbContext.ReadMediaAssets
            .FirstOrDefaultAsync(ma => ma.Id == query.MediaAssetId, cancellationToken);

        if (mediaAssetResult == null)
            return GeneralErrors.EntityNotFound("Media asset").ToErrors();

        var downloadResult = await _s3Provider.DownloadFileAsync(mediaAssetResult.FinalKey, PATH_TO_DOWNLOAD, cancellationToken);
        if (downloadResult.IsFailure)
        {
            _logger.LogError("Errors occurred when downloading file with id {Id}", query.MediaAssetId);
            return downloadResult.Error.ToErrors();
        }

        return downloadResult.Value;
    }
}