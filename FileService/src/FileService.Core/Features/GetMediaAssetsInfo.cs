using CSharpFunctionalExtensions;
using FileService.Contracts.Dtos;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Core.Abstractions.Database;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SharedService.Core.Abstractions;
using SharedService.Framework.Endpoints;
using SharedService.SharedKernel;

namespace FileService.Core.Features;

public record GetMediaAssetsInfoQuery(GetMediaAssetsInfoRequest GetMediaAssetsInfoRequest) : IQuery;

public class GetMediaAssetsInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("batch", async Task<EndpointResult<GetMediaAssetsInfoResponse>>(
            [FromBody] GetMediaAssetsInfoRequest request,
            [FromServices] GetMediaAssetsInfoHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new GetMediaAssetsInfoQuery(request), cancellationToken));
    }
}

public class GetMediaAssetsInfoHandler : IQueryHandler<Result<GetMediaAssetsInfoResponse, Errors>, GetMediaAssetsInfoQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IReadFileServiceDbContext _readDbContext;

    public GetMediaAssetsInfoHandler(
        IS3Provider s3Provider,
        IReadFileServiceDbContext readDbContext)
    {
        _s3Provider = s3Provider;
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetMediaAssetsInfoResponse, Errors>> Handle(
        GetMediaAssetsInfoQuery query,
        CancellationToken cancellationToken)
    {
        if (!query.GetMediaAssetsInfoRequest.MediaAssetIds.Any())
            return new GetMediaAssetsInfoResponse([]);

        var mediaAssets = await _readDbContext.ReadMediaAssets
            .Where(mA => query.GetMediaAssetsInfoRequest.MediaAssetIds.Contains(mA.Id)
                   && mA.MediaStatus != MediaStatus.DELETED)
            .ToListAsync(cancellationToken);
        if (!mediaAssets.Any())
            return GeneralErrors.EntityNotFound("Media assets").ToErrors();

        var keys = mediaAssets
            .Where(mA => mA.MediaStatus == MediaStatus.READY)
            .Select(rMA => rMA.RawKey)
            .ToList();

        var downloadUrlsResult = await _s3Provider.GenerateDownloadUrlsAsync(keys, cancellationToken);
        if (downloadUrlsResult.IsFailure)
            return downloadUrlsResult.Error.ToErrors();

        var urls = downloadUrlsResult.Value.ToDictionary(uU => uU.StorageKey, uU => uU.PresignedUrl);

        var mediaAssetDtos = new List<MediaAssetsDto>();
        foreach (var mA in mediaAssets)
        {
            urls.TryGetValue(mA.RawKey, out string? url);
            var mediaAssetDto = new MediaAssetsDto(
                mA.Id,
                mA.MediaStatus.ToString().ToLowerInvariant(),
                url);

            mediaAssetDtos.Add(mediaAssetDto);
        }

        return new GetMediaAssetsInfoResponse(mediaAssetDtos);
    }
}