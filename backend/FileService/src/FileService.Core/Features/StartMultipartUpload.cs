using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using FileService.Core.Abstractions.Database;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain;
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

public record StartMultipartUploadCommand(StartMultipartUploadRequest StartMultipartUploadRequest) : ICommand;

public class StartMultipartUploadValidator : AbstractValidator<StartMultipartUploadCommand>
{
    public StartMultipartUploadValidator()
    {
        RuleFor(command => command.StartMultipartUploadRequest)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Start multipart upload command"));

        RuleFor(command => command.StartMultipartUploadRequest.FileName)
            .MustBeValueObject(fN => ResultsExtensions.ToErrorsResult(FileName.Create(fN)));

        RuleFor(command => command.StartMultipartUploadRequest.ContentType)
            .MustBeValueObject(fN => ResultsExtensions.ToErrorsResult(ContentType.Create(fN)));

        RuleFor(command => command.StartMultipartUploadRequest.AssetType)
            .Must(aT =>
            {
                if (string.IsNullOrEmpty(aT))
                    return false;

                return Enum.TryParse<AssetType>(aT, true, out _);
            })
                .WithError(GeneralErrors.ValueIsNotValid("AssetType must be a valid asset type"));

        RuleFor(command => command.StartMultipartUploadRequest)
            .MustBeValueObject(sMR => ResultsExtensions.ToErrorsResult(MediaOwner.Create(sMR.ContextId, sMR.Context)));
    }
}

public class StartMultipartUploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("multipart/start", async Task<EndpointResult<StartMultipartUploadResponse>>(
            [FromBody] StartMultipartUploadRequest request,
            [FromServices] StartMultipartUploadHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new StartMultipartUploadCommand(request), cancellationToken));
    }
}

public class StartMultipartUploadHandler : ICommandHandler<StartMultipartUploadResponse, StartMultipartUploadCommand>
{
    private readonly IS3Provider _s3Provider;
    private readonly IChunkSizeCalculator _chunkSizeCalculator;
    private readonly IMediaAssetFactory _mediaAssetFactory;
    private readonly ITransactionManager _transactionManager;
    private readonly IMediaRepository _mediaRepository;
    private readonly IValidator<StartMultipartUploadCommand> _validator;
    private readonly ILogger<StartMultipartUploadHandler> _logger;

    public StartMultipartUploadHandler(
        IS3Provider s3Provider,
        IChunkSizeCalculator chunkSizeCalculator,
        IMediaAssetFactory mediaAssetFactory,
        ITransactionManager transactionManager,
        IMediaRepository mediaRepository,
        IValidator<StartMultipartUploadCommand> validator,
        ILogger<StartMultipartUploadHandler> logger)
    {
        _s3Provider = s3Provider;
        _chunkSizeCalculator = chunkSizeCalculator;
        _mediaAssetFactory = mediaAssetFactory;
        _mediaRepository = mediaRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<StartMultipartUploadResponse, Errors>> Handle(
        StartMultipartUploadCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating UploadFileCommand");
            return validationResult.ToErrors();
        }

        var request = command.StartMultipartUploadRequest;

        var fileName = FileName.Create(request.FileName).Value;
        var contentType = ContentType.Create(request.ContentType).Value;

        var chunkCalculateResult = _chunkSizeCalculator.Calculate(request.Size);
        if (chunkCalculateResult.IsFailure)
            return chunkCalculateResult.Error.ToErrors();

        var mediaDataResult = MediaData.Create(fileName, contentType, request.Size, chunkCalculateResult.Value.TotalChunks);
        if (mediaDataResult.IsFailure)
            return mediaDataResult.Error.ToErrors();

        var mediaOwner = MediaOwner.Create(request.ContextId, request.Context).Value;
        var assetType = Enum.Parse<AssetType>(request.AssetType, true);

        var mediaAssetResult = _mediaAssetFactory.CreateForUpload(mediaDataResult.Value, assetType, mediaOwner);
        if (mediaAssetResult.IsFailure)
            return mediaAssetResult.Error.ToErrors();

        var startMultipartUploadResult = await _s3Provider.StartMultipartUploadAsync(
            mediaAssetResult.Value.RawKey,
            mediaDataResult.Value,
            cancellationToken);
        if (startMultipartUploadResult.IsFailure)
            return startMultipartUploadResult.Error.ToErrors();

        var uploadId = startMultipartUploadResult.Value;

        var addingResult = await _mediaRepository.AddAsync(mediaAssetResult.Value, cancellationToken);
        if (addingResult.IsFailure)
        {
            _logger.LogError("Errors occurred when adding media asset");
            return addingResult.Error.ToErrors();
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveChangesResult.Error.ToErrors();
        }

        var generateAllChunkUploadedUrlsResult = await _s3Provider.GenerateAllChunkUploadUrlsAsync(
            mediaAssetResult.Value.RawKey,
            uploadId,
            mediaDataResult.Value.ExpectedChuncksCount,
            cancellationToken);

        if (generateAllChunkUploadedUrlsResult.IsFailure)
        {
            _logger.LogError("Errors occurred when generating all chunk uploaded urls");
            return generateAllChunkUploadedUrlsResult.Error.ToErrors();
        }

        return new StartMultipartUploadResponse(
            mediaAssetResult.Value.Id,
            uploadId,
            generateAllChunkUploadedUrlsResult.Value,
            chunkCalculateResult.Value.ChunkSize);
    }
}