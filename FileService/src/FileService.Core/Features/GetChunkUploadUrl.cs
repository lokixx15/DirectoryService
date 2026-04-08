using CSharpFunctionalExtensions;
using FileService.Contracts.Dtos;
using FileService.Contracts.Requests;
using FileService.Core.Abstractions.Database;
using FileService.Core.Abstractions.FileStorage;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.Framework.Endpoints;
using SharedService.SharedKernel;

namespace FileService.Core.Features;

public record GetChunkUploadUrlQuery(GetChunkUploadUrlRequest GetChunkUploadUrlRequest) : IQuery;

public class GetChunkUploadUrlValidator : AbstractValidator<GetChunkUploadUrlQuery>
{
    public GetChunkUploadUrlValidator()
    {
        RuleFor(command => command.GetChunkUploadUrlRequest)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.GetChunkUploadUrlRequest.UploadId)
           .NotEmpty()
               .WithError(GeneralErrors.ValueIsNotValid("Upload id cannot be empty"));

        RuleFor(command => command.GetChunkUploadUrlRequest.PartNumber)
            .GreaterThan(0)
                .WithError(GeneralErrors.ValueIsNotValid("Part number must be greater than 0"));
    }
}

public class GetChunkUploadUrlEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("multipart/url", async Task<EndpointResult<ChunkUploadUrlDto>>(
            [FromBody] GetChunkUploadUrlRequest request,
            [FromServices] GetChunkUploadUrlHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new GetChunkUploadUrlQuery(request), cancellationToken));
    }
}

public class GetChunkUploadUrlHandler : IQueryHandler<Result<ChunkUploadUrlDto, Errors>, GetChunkUploadUrlQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IMediaRepository _mediaRepository;
    private readonly IValidator<GetChunkUploadUrlQuery> _validator;
    private readonly ILogger<GetChunkUploadUrlHandler> _logger;

    public GetChunkUploadUrlHandler(
        IS3Provider s3Provider,
        IMediaRepository mediaRepository,
        IValidator<GetChunkUploadUrlQuery> validator,
        ILogger<GetChunkUploadUrlHandler> logger)
    {
        _s3Provider = s3Provider;
        _mediaRepository = mediaRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<ChunkUploadUrlDto, Errors>> Handle(
        GetChunkUploadUrlQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating GetChunkUploadUrlQuery");
            return validationResult.ToErrors();
        }

        var request = query.GetChunkUploadUrlRequest;

        var mediaAssetResult = await _mediaRepository.GetByAsync(mA => mA.Id == request.MediaAssetId, cancellationToken);
        if (mediaAssetResult.IsFailure)
        {
            _logger.LogError("Media asset with id {Id} was not found", request.MediaAssetId);
            return GeneralErrors.EntityNotFound("Media asset").ToErrors();
        }

        var urlResult = await _s3Provider.GenerateChunkUploadUrlAsync(mediaAssetResult.Value.RawKey, request.UploadId, request.PartNumber);
        if (urlResult.IsFailure)
        {
            _logger.LogError("Errors occurred when generating chunk upload url for media asset with id {Id}", request.MediaAssetId);
            return urlResult.Error.ToErrors();
        }

        return urlResult.Value;
    }
}