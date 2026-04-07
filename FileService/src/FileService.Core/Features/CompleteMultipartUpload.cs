using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
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

public record CompleteMultipartUploadCommand(CompleteMultipartUploadRequest CompleteMultipartUploadRequest) : ICommand;

public class CompleteMultipartUploadValidator : AbstractValidator<CompleteMultipartUploadCommand>
{
    public CompleteMultipartUploadValidator()
    {
        RuleFor(command => command.CompleteMultipartUploadRequest)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.CompleteMultipartUploadRequest.UploadId)
           .NotEmpty()
               .WithError(GeneralErrors.ValueIsNotValid("Upload id cannot be empty"));

        RuleFor(command => command.CompleteMultipartUploadRequest.PartETags)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNotValid("PartETags are required"));
    }
}

public class CompleteMultipartUploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("multipart/complete", async Task<EndpointResult<CompleteMultipartUploadResponse>>(
            [FromBody] CompleteMultipartUploadRequest request,
            [FromServices] CompleteMultipartUploadHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new CompleteMultipartUploadCommand(request), cancellationToken));
    }
}

public class CompleteMultipartUploadHandler : ICommandHandler<CompleteMultipartUploadResponse, CompleteMultipartUploadCommand>
{
    private readonly IS3Provider _s3Provider;
    private readonly ITransactionManager _transactionManager;
    private readonly IMediaRepository _mediaRepository;
    private readonly IValidator<CompleteMultipartUploadCommand> _validator;
    private readonly ILogger<CompleteMultipartUploadHandler> _logger;

    public CompleteMultipartUploadHandler(
        IS3Provider s3Provider,
        ITransactionManager transactionManager,
        IMediaRepository mediaRepository,
        IValidator<CompleteMultipartUploadCommand> validator,
        ILogger<CompleteMultipartUploadHandler> logger)
    {
        _s3Provider = s3Provider;
        _mediaRepository = mediaRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<CompleteMultipartUploadResponse, Errors>> Handle(
        CompleteMultipartUploadCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating UploadFileCommand");
            return validationResult.ToErrors();
        }

        var request = command.CompleteMultipartUploadRequest;

        var mediaAssetResult = await _mediaRepository.GetByAsync(mA => mA.Id == request.MediaAssetId, cancellationToken);
        if (mediaAssetResult.IsFailure)
            return mediaAssetResult.Error.ToErrors();

        var mediaAsset = mediaAssetResult.Value;

        if (request.PartETags.Count != mediaAsset.MediaData.ExpectedChuncksCount)
            return GeneralErrors.ValueIsNotValid("Number etags must match the number of expected chuncks").ToErrors();

        var completeMultipartUpload = await _s3Provider.CompleteMultipartUploadAsync(
            mediaAsset.RawKey,
            request.UploadId,
            request.PartETags,
            cancellationToken);
        if (completeMultipartUpload.IsFailure)
        {
            var markFailedResult = mediaAsset.MarkFailed(DateTime.UtcNow);
            if (markFailedResult.IsFailure)
            {
                _logger.LogError("Errors occurred when marking media asset as failed");
                return markFailedResult.Error.ToErrors();
            }

            var saveFailedResult = await _transactionManager.SaveChangesAsync(cancellationToken);
            if (saveFailedResult.IsFailure)
            {
                _logger.LogError("Errors occurred when saving changes");
                return saveFailedResult.Error.ToErrors();
            }

            return completeMultipartUpload.Error.ToErrors();
        }

        var markUploadedResult = mediaAsset.MarkUploaded(DateTime.UtcNow);
        if (markUploadedResult.IsFailure)
        {
            _logger.LogError("Errors occurred when marking media asset as uploaded");
            return markUploadedResult.Error.ToErrors();
        }

        var saveUploadedResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveUploadedResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveUploadedResult.Error.ToErrors();
        }

        return new CompleteMultipartUploadResponse(mediaAsset.Id);
    }
}