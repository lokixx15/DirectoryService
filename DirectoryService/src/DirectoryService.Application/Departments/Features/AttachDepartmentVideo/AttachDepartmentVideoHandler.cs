using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions.Database;
using FileService.Communication;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.AttachDepartmentVideo;

public class AttachDepartmentVideoHandler : ICommandHandler<AttachDepartmentVideoCommand>
{
    private readonly IDepartmentsRepository _departmentRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AttachDepartmentVideoHandler> _logger;
    private readonly IFileService _fileService;

    public AttachDepartmentVideoHandler(
        IDepartmentsRepository departmentRepository,
        ITransactionManager transactionManager,
        ILogger<AttachDepartmentVideoHandler> logger,
        IFileService fileService)
    {
        _departmentRepository = departmentRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _fileService = fileService;
    }

    public async Task<UnitResult<Errors>> Handle(AttachDepartmentVideoCommand command, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentRepository
            .GetByAsync(d => d.Id == command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
        {
            _logger.LogError("Failed to get department with id {DepartmentId}", command.DepartmentId);
            return departmentResult.Error.ToErrors();
        }

        var videoExistsResult = await _fileService.CheckVideoExistence(command.Request.VideoId, cancellationToken);
        if (videoExistsResult.IsFailure)
        {
            _logger.LogError("Failed to check video existence with id {VideoId}", command.Request.VideoId);
            return videoExistsResult.Error.ToErrors();
        }

        departmentResult.Value.AttachVideo(command.Request.VideoId);
        _logger.LogInformation("Video with id {VideoId} has been attached to department with id {DepartmentId}", command.Request.VideoId, command.DepartmentId);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Errors>();
    }
}