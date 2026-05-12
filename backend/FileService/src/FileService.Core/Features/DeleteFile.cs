using CSharpFunctionalExtensions;
using FileService.Core.Abstractions.Database;
using FileService.Core.Abstractions.FileStorage;
using FileService.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Framework.Endpoints;
using SharedService.SharedKernel;

namespace FileService.Core.Features;

public record DeleteFileQuery(Guid MediaAssetId) : ICommand;

public class DeleteFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("{mediaAssetId:guid}", async Task<EndpointResult<string>>(
            [FromRoute] Guid mediaAssetId,
            [FromServices] DeleteFileHandler handler,
            CancellationToken cancellationToken) =>
        await handler.Handle(new DeleteFileQuery(mediaAssetId), cancellationToken));
    }
}

public class DeleteFileHandler : ICommandHandler<string, DeleteFileQuery>
{
    private readonly IS3Provider _s3Provider;
    private readonly IMediaRepository _mediadRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DeleteFileHandler> _logger;

    public DeleteFileHandler(
        IS3Provider s3Provider,
        IMediaRepository mediadRepository,
        ITransactionManager transactionManager,
        ILogger<DeleteFileHandler> logger)
    {
        _s3Provider = s3Provider;
        _mediadRepository = mediadRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<string, Errors>> Handle(
        DeleteFileQuery query,
        CancellationToken cancellationToken)
    {
        if (query.MediaAssetId == Guid.Empty)
            return GeneralErrors.ValueIsNotValid("Guid cannot be empty", nameof(query.MediaAssetId)).ToErrors();

        var mediaAssetResult = await _mediadRepository.GetByAsync(
            ma => ma.Id == query.MediaAssetId
            && ma.MediaStatus != MediaStatus.DELETED, cancellationToken);
        if (mediaAssetResult.IsFailure)
            return mediaAssetResult.Error.ToErrors();

        var mediaAsset = mediaAssetResult.Value;

        var downloadResult = await _s3Provider.DeleteFileAsync(mediaAsset.RawKey, cancellationToken);
        if (downloadResult.IsFailure)
        {
            _logger.LogError("Errors occurred when deleting file with id {Id}", query.MediaAssetId);
            return downloadResult.Error.ToErrors();
        }

        var markDeletedResult = mediaAsset.MarkDeleted(DateTime.UtcNow);
        if (markDeletedResult.IsFailure)
            return markDeletedResult.Error.ToErrors();

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            _logger.LogError("Errors occurred when saving changes");
            return saveChangesResult.Error.ToErrors();
        }

        return downloadResult.Value;
    }
}