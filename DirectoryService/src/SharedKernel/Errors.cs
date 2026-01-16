using System.Collections;

namespace SharedKernel;

public class Errors : IEnumerable<Error>
{
    private readonly List<Error> _errors;

    public Errors(IEnumerable<Error> errors)
    {
        _errors = [..errors];
    }

    public Errors(IEnumerable<Errors> errors)
    {
        _errors = [.. errors.SelectMany(e => e)];
    }

    public IEnumerator<Error> GetEnumerator() 
    { 
        return _errors.GetEnumerator(); 
    }

    IEnumerator IEnumerable.GetEnumerator() => _errors.GetEnumerator();

    public static implicit operator Errors(Error error) => new Errors([error]);

    public static implicit operator Errors(List<Error> errors) => new Errors(errors);
}