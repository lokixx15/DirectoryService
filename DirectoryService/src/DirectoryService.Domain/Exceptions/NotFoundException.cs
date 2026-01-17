using SharedKernel;

namespace DirectoryService.Domain.Exceptions;

public class NotFoundException : Exception
{
    public Error Error { get; } = null!;
    public NotFoundException(Error error) : base(error.GetMessage()) 
    {
        Error = error;
    }
}