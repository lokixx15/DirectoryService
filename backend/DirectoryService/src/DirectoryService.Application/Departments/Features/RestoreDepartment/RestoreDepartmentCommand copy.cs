using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.SharedKernel;

namespace DirectoryService.Application.Departments.Features.RestoreDepartment;

public class RestoreDepartmentHandler : ICommandHandler<RestoreDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILogger<RestoreDepartmentHandler> _logger;

    public RestoreDepartmentHandler(
        IDepartmentsRepository departmentsRepository,
        ILogger<RestoreDepartmentHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
    }

    public async Task<UnitResult<Errors>> Handle(RestoreDepartmentCommand command, CancellationToken cancellationToken)
    {
        var restoreDepartmentResult = await _departmentsRepository.RestoreDepartmentByIdAsync(command.Id, cancellationToken);
        if (restoreDepartmentResult.IsFailure)
        {
            _logger.LogError("Errors occured when restoring department");
            return restoreDepartmentResult.Error.ToErrors();
        }

        return UnitResult.Success<Errors>();
    }
}
