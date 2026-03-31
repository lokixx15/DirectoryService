using CSharpFunctionalExtensions;
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

public record AbortMultipartUploadCommand(AbortMultipartUploadRequest AbortMultipartUploadRequest) : ICommand;

public class AbortMultipartUploadValidator : AbstractValidator<AbortMultipartUploadCommand>
{
    public AbortMultipartUploadValidator()
    {
        RuleFor(command => command.AbortMultipartUploadRequest)
            .NotNull()
                .WithError(GeneralErrors.ValueIsNullOrWhitespace("Request"));

        RuleFor(command => command.AbortMultipartUploadRequest.UploadId)
            .Must(uI =>
            {
                if (string.IsNullOrEmpty(uI))
                    return false;

                return Guid.TryParse(uI, out _);
            })
                .WithError(GeneralErrors.ValueIsNotValid("Upload id must be a guid"));
    }
}

public class AbortMultipartUploadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("multipart/abort", async Task<EndpointResult<AbortMultipartUploadResponse>>(
            [FromBody] AbortMultipartUploadRequest request,
            [FromServices] AbortMultipartUploadHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new AbortMultipartUploadCommand(request), cancellationToken));
    }
}

public class AbortMultipartUploadHandler : ICommandHandler<AbortMultipartUploadResponse, AbortMultipartUploadCommand>
{
    private readonly IS3Provider _s3Provider;
    private readonly ITransactionManager _transactionManager;
    private readonly IMediaRepository _mediaRepository;
    private readonly IValidator<AbortMultipartUploadCommand> _validator;
    private readonly ILogger<AbortMultipartUploadHandler> _logger;

    public AbortMultipartUploadHandler(
        IS3Provider s3Provider,
        ITransactionManager transactionManager,
        IMediaRepository mediaRepository,
        IValidator<AbortMultipartUploadCommand> validator,
        ILogger<AbortMultipartUploadHandler> logger)
    {
        _s3Provider = s3Provider;
        _mediaRepository = mediaRepository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<AbortMultipartUploadResponse, Errors>> Handle(
        AbortMultipartUploadCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Errors occurred when validating AbortMultipartUploadCommand");
            return validationResult.ToErrors();
        }

        var request = command.AbortMultipartUploadRequest;

        var mediaAssetResult = await _mediaRepository.GetByAsync(mA => mA.Id == request.MediaAssetId, cancellationToken);
        if (mediaAssetResult.IsFailure)
        {
            _logger.LogError("Media asset with id {MediaAssetId} is not found", request.MediaAssetId);
            return mediaAssetResult.Error.ToErrors();
        }

        var abortMultipartUploadResult = await _s3Provider.AbortMultipartUploadAsync(
            mediaAssetResult.Value.RawKey,
            request.UploadId,
            cancellationToken);
        if (abortMultipartUploadResult.IsFailure)
        {
            _logger.LogError("Errors occurred when aborting multipart upload for media asset with id {MediaAssetId}", request.MediaAssetId);
            return abortMultipartUploadResult.Error.ToErrors();
        }

        var markFailedResult = mediaAssetResult.Value.MarkFailed(DateTime.UtcNow);
        if (markFailedResult.IsFailure) {
            _logger.LogError("Errors occurred when marking media asset with id {MediaAssetId} as failed", request.MediaAssetId);
            return markFailedResult.Error.ToErrors();
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveChangesResult.Error.ToErrors();
        }

        return new AbortMultipartUploadResponse(true);
    }
}