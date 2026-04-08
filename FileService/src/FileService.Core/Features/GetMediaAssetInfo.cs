using CSharpFunctionalExtensions;
using FileService.Contracts.Dtos;
using FileService.Contracts.Responses;
using FileService.Core.Abstractions.Database;
using FileService.Core.Abstractions.FileStorage;
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

public record GetMediaAssetInfoQuery(Guid MediaAssetId) : IQuery;

public class GetMediaAssetInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{mediaAssetId:guid}/info", async Task<EndpointResult<GetMediaAssetInfoResponse>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] GetMediaAssetInfoHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new GetMediaAssetInfoQuery(mediaAssetId), cancellationToken));
    }
}

public class GetMediaAssetInfoHandler : IQueryHandler<Result<GetMediaAssetInfoResponse, Errors>, GetMediaAssetInfoQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IReadFileServiceDbContext _readDbContext;

    public GetMediaAssetInfoHandler(
        IS3Provider s3Provider,
        IReadFileServiceDbContext readDbContext,
        ILogger<GetDownloadUrlHandler> logger)
    {
        _s3Provider = s3Provider;
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetMediaAssetInfoResponse, Errors>> Handle(
        GetMediaAssetInfoQuery query,
        CancellationToken cancellationToken)
    {
        if (query.MediaAssetId == Guid.Empty)
            return GeneralErrors.ValueIsNotValid("Guid cannot be empty", nameof(query.MediaAssetId)).ToErrors();

        var mediaAsset = await _readDbContext.ReadMediaAssets
            .FirstOrDefaultAsync(mA => query.MediaAssetId == mA.Id && mA.MediaStatus != MediaStatus.DELETED, cancellationToken);
        if (mediaAsset == null)
            return GeneralErrors.EntityNotFound("Media asset").ToErrors();

        var fileInfoDto = new FileInfoDto(
            mediaAsset.MediaData.FileName.Value,
            mediaAsset.MediaData.ContentType.Value,
            mediaAsset.MediaData.Size);

        var mediaAssetDto = new MediaAssetDto(
            mediaAsset.Id,
            mediaAsset.MediaStatus.ToString().ToLowerInvariant(),
            mediaAsset.AssetType.ToString().ToLowerInvariant(),
            mediaAsset.CreatedAt,
            mediaAsset.UpdatedAt,
            fileInfoDto);

        string? url = null;
        if (mediaAsset.MediaStatus == MediaStatus.READY)
        {
            var downloadUrlResult = await _s3Provider.GenerateDownloadUrlAsync(mediaAsset.RawKey);
            if (downloadUrlResult.IsFailure)
                return downloadUrlResult.Error.ToErrors();

            url = downloadUrlResult.Value;
        }

        return new GetMediaAssetInfoResponse(mediaAssetDto, url);
    }
}