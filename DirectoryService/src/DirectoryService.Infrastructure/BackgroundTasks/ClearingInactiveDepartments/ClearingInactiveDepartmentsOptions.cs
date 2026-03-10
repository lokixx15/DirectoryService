namespace DirectoryService.Infrastructure.BackgroundTasks.ClearingInactiveDepartments;

public class ClearingInactiveDepartmentsOptions
{
    public TimeSpan CleaningInterval { get; set; }

    public TimeSpan MaxLifeCycleOfRemoteDepartment { get; set; }
}