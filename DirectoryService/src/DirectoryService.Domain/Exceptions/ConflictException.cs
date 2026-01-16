using SharedKernel;

namespace DirectoryService.Domain.Exceptions;

public class ConflictException : Exception
{
    public Error Error { get; } = null!;
    public ConflictException(Error error) : base(error.GetMessage()) 
    { 
        Error = error;
    }
}