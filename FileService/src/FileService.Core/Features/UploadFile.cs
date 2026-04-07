using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
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

public record UploadFileCommand(UploadFileRequest UploadFileRequest) : ICommand;

public class UploadFileValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileValidator()
    {
        RuleFor(command => command.UploadFileRequest)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Upload file command"));

        RuleFor(command => command.UploadFileRequest.FormFile)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNotValid("File is required"));

        RuleFor(command => command.UploadFileRequest.FormFile.FileName)
            .MustBeValueObject(fN => ResultsExtensions.ToErrorsResult(FileName.Create(fN)));

        RuleFor(command => command.UploadFileRequest.AssetType)
            .Must(aT =>
            {
                if (string.IsNullOrEmpty(aT))
                    return false;

                return Enum.TryParse<AssetType>(aT, true, out _);
            })
                .WithError(GeneralErrors.ValueIsNotValid("AssetType must be a valid asset type"));

        RuleFor(command => command.UploadFileRequest.FormFile.ContentType)
            .MustBeValueObject(cT => ResultsExtensions.ToErrorsResult(ContentType.Create(cT)));

        RuleFor(command => command.UploadFileRequest)
            .MustBeValueObject(uFR => ResultsExtensions.ToErrorsResult(MediaOwner.Create(uFR.EntityId, uFR.Context)));
    }
}

public class UploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async Task<EndpointResult>(
            [FromForm] UploadFileRequest request,
            [FromServices] UploadFileHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new UploadFileCommand(request), cancellationToken))
            .DisableAntiforgery();
    }
}

public class UploadFileHandler : ICommandHandler<UploadFileCommand>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IMediaAssetFactory _mediaAssetFactory;
    private readonly IS3Provider _s3Provider;
    private readonly IValidator<UploadFileCommand> _validator;
    private readonly ILogger<UploadFileHandler> _logger;

    public UploadFileHandler(
        IMediaRepository mediaRepository,
        ITransactionManager transactionManager,
        IMediaAssetFactory mediaAssetFactory,
        IS3Provider s3Provider,
        IValidator<UploadFileCommand> validator,
        ILogger<UploadFileHandler> logger)
    {
        _mediaRepository = mediaRepository;
        _transactionManager = transactionManager;
        _mediaAssetFactory = mediaAssetFactory;
        _s3Provider = s3Provider;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(
        UploadFileCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating UploadFileCommand");
            return validationResult.ToErrors();
        }

        var request = command.UploadFileRequest;
        var formFile = request.FormFile;

        var fileName = FileName.Create(formFile.FileName).Value;
        var contentType = ContentType.Create(formFile.ContentType).Value;

        var mediaDataResult = MediaData.Create(fileName, contentType, formFile.Length, 1);
        if (mediaDataResult.IsFailure)
            return mediaDataResult.Error.ToErrors();

        var mediaOwner = MediaOwner.Create(request.EntityId, request.Context).Value;
        var assetType = Enum.Parse<AssetType>(request.AssetType, true);

        var mediaAssetResult = _mediaAssetFactory.CreateForUpload(mediaDataResult.Value, assetType, mediaOwner);
        if (mediaAssetResult.IsFailure)
            return mediaAssetResult.Error.ToErrors();

        var mediaAsset = mediaAssetResult.Value;

        var uploadResult = await _s3Provider.UploadFileAsync(
            mediaAsset.RawKey,
            formFile.OpenReadStream(),
            mediaDataResult.Value,
            cancellationToken);

        if (uploadResult.IsFailure)
            return uploadResult.Error.ToErrors();

        var markUploadedResult = mediaAsset.MarkUploaded(DateTime.UtcNow);
        if (markUploadedResult.IsFailure)
            return markUploadedResult.Error.ToErrors();

        var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);
        if(beginTransactionResult.IsFailure)
            return beginTransactionResult.Error.ToErrors();

        var transactionScope = beginTransactionResult.Value;

        var addingResult = await _mediaRepository.AddAsync(mediaAsset, cancellationToken);
        if (addingResult.IsFailure)
        {
            _logger.LogError("Errors occurred when adding media asset");
            return addingResult.Error.ToErrors();
        }

        var completeUploadResult = mediaAsset.CompleteProcessing(DateTime.UtcNow);
        if (completeUploadResult.IsFailure)
        {
            transactionScope.Rollback();
            return completeUploadResult.Error.ToErrors();
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveChangesResult.Error.ToErrors();
        }

        var commitResult = transactionScope.Commit();
        if (commitResult.IsFailure)
        {
            _logger.LogError("Errors occurred when committing transaction");
            return saveChangesResult.Error.ToErrors();
        }

        _logger.LogInformation("File was uploaded successfully");
        return UnitResult.Success<Errors>();
    }
}